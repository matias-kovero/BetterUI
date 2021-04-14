using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.Patches
{
  static class CustomHud
  {
    public static List<HudElement> elements;
    public static Dictionary<Groups, Transform> roots = new Dictionary<Groups, Transform>();
    public static Dictionary<Groups, Transform> templates = new Dictionary<Groups, Transform>();
    private static Transform hudRoot;
    private static Transform invRoot;
    private static Transform baseRoot;
    public static readonly string templateSuffix = "_template";

    // In the future give users ability to add new elements?
    private static readonly Element[] supportedElements =
    {
      new Element("HotKeyBar", Groups.HudRoot),
      new Element("BuildHud", Groups.HudRoot, "BuildHud/SelectedInfo"),
      new Element("MiniMap", Groups.HudRoot, "MiniMap/small"),
      new Element("GuardianPower", Groups.HudRoot),
      new Element("StatusEffects", Groups.HudRoot),
      new Element("SaveIcon", Groups.HudRoot),
      new Element("BadConnectionIcon", Groups.HudRoot),
      new Element("BuildHints", Groups.HudRoot, "KeyHints/BuildHints"),
      new Element("CombatHints", Groups.HudRoot, "KeyHints/CombatHints"),
      new Element("Player", Groups.Inventory, "Player", "PlayerInventory"),
      new Element("Container", Groups.Inventory, "Container", "ChestContainer"),
      new Element("Info", Groups.Inventory, "Info", "UITab"),
      new Element("Crafting", Groups.Inventory, "Crafting", "CraftingWindow"),
      new Element(CustomElements.HealthBar.objectName, Groups.HudRoot, CustomElements.HealthBar.objectName, "HP Bar"),
      new Element(CustomElements.FoodBar.objectName, Groups.HudRoot, CustomElements.FoodBar.objectName, "Food Bar"),
      new Element(CustomElements.StaminaBar.objectName, Groups.HudRoot, CustomElements.StaminaBar.objectName, "Stamina Bar"),
      new Element("QuickSlots", Groups.HudRoot, "QuickSlotsHotkeyBar", "QuickSlots")
      //new Element("QuickSlotsHotkeyBar", Groups.HudRoot, "healthpanel/Health/QuickSlotsHotkeyBar", "QuickSlotsHotkey"),
      //new Element("QuickSlotGrid", Groups.Inventory, "Player/QuickSlotGrid", "QuickSlots"),
      //new Element("EquipmentSlotGrid", Groups.Inventory, "Player/EquipmentSlotGrid", "EquipmentSlots"),
    };

    // If new items are added to mandatory items, check if user has them - if not add them.
    public static void Load(Hud hud)
    {
      try
      {
        hudRoot = hud.transform.Find("hudroot");
        invRoot = InventoryGui.instance.transform.Find("root"); // Issue, this element is hidden when inventory is closed
        baseRoot = MessageHud.instance.transform; // This layer will be projected over other UI elements

        roots[Groups.HudRoot] = hudRoot;
        roots[Groups.Inventory] = invRoot;


        if (Main.uiData.Value == "none" || Main.uiData.Value == "")
        {
          Helpers.DebugLine($"User has no uiData. Creating basic template.");
          elements = new List<HudElement>();
        }
        else
        {
          try
          {
            byte[] bytes = Convert.FromBase64String(Main.uiData.Value);
            elements = (List<HudElement>)bytes.DeSerialize(); // Risky, as we trust the data is valid?
            Helpers.DebugLine($"User has {elements.Count} ui elements set.");
          }
          catch
          {
            Helpers.DebugLine($"FAILED to DeSerialize uiData: {Main.uiData.Value}");
          }
        }

        if (elements.Count < supportedElements.Length)
        {
          foreach (Element e in supportedElements)
          {
            // Element does not exist in users uiData, add it.
            if (!elements.Exists(he => he.name == e.name))
            {
              Helpers.DebugLine($"Adding to elements: {e.name}");
              elements.Add(new HudElement(e.name, Vector2.zero, 1f, 1f, e.displayName, e.group, e.locationPath));

              if (elements.Count == supportedElements.Length) break;
            }
          }
        }
        else if (elements.Count > supportedElements.Length)
        {
          // We have more elements than supported? Are there duplicates, how?
          Helpers.DebugLine($"Seems that your UI might be corrupted!", true, true);
        }

        CreateTemplates();
      }
      catch (Exception e)
      {
        Helpers.DebugLine($"Issue while CustomHud Load. {e.Message}", true, true);
      }
    }

    public static void Save()
    {
      try
      {
        // Before saving, check if unset elements -> no need to save them.
        elements.RemoveAll(e => e.GetPosition() == Vector2.zero);
        byte[] bytes = elements.Serialize();
        Helpers.DebugLine($"uiData bytes: {bytes.Length}");
        string base64String = Convert.ToBase64String(bytes);
        Main.uiData.Value = base64String;
      }
      catch (Exception e)
      {
        Helpers.DebugLine($"FAILED to Save: {e.Message}");
      }
    }

    public static void ShowTemplates(bool show, int activeLayer)
    {
      // Try to find reason on using this?
      roots.TryGetValue((Groups)activeLayer, out Transform activeTemplate);

      foreach (HudElement e in elements)
      {
        if ((Groups)activeLayer == e.group)
        {
          RectTransform rt = LocateTemplateRect(e.group, e.name);
          if (rt)
          {
            rt.gameObject.SetActive(show);
          }
        }
        else
        {
          RectTransform rt = LocateTemplateRect(e.group, e.name);
          if (rt)
          {
            rt.gameObject.SetActive(false);
          }
        }
      }


      if (!show) Save();
    }

    public static void UpdatePosition(string name, Vector3 pos, float size)
    {
      HudElement element = elements.Find(e => e.name == name);
      if (element.name == name)
      {
        element.x += pos.x;
        element.y += pos.y;
        if (size != 0f)
        {
          element.SetScale(size);
          Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"{element.displayName} size: {element.scale}");
        }
        if (element.group == Groups.Inventory)
        {
          Vector3 newPos = Camera.main.ScreenToViewportPoint(new Vector3(pos.x, pos.y, pos.z));
          //element.SetAnchors(newPos, newPos);
          //element.anchorMin += new Vector2(newPos.x, newPos.y);
          element.UpdateAnchors(newPos, newPos);
          //element.anchorMax += new Vector2(newPos.x, newPos.y);
        }
        // Update element & template position
        PositionTemplate(element);
      }
    }

    // This creates issues with Text. Scaling is off!
    public static void UpdateDimensions(string name, float size)
    {
      HudElement element = elements.Find(e => e.name == name);
      if (element.name == name)
      {
        if (size != 0f)
        {
          element.SetDims(size);
          Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"{element.displayName} dimensions: (1,{element.dimensions})");
          // Update element & template position
          PositionTemplate(element);
        }
      }
      else
      {
        Helpers.DebugLine($"Invalid call when updating element: {name}", true, true);
      }
    }

    private static void PositionTemplate(HudElement e)
    {
      try
      {
        RectTransform rt = LocateRectTransform(e.group, e.path);  // Original object
        RectTransform tt = LocateTemplateRect(e.group, e.name);   // Your generated template
        //Helpers.DebugLine($"{rt} {rt.anchorMin} {e.GetPosition()}");
        if (rt)
        {
          if (e.group == Groups.Inventory)
          {
            float gameScale = GameObject.Find("GUI").GetComponent<CanvasScaler>().scaleFactor;
            //Helpers.DebugLine($"\n{e.GetPosition()}\n{gameScale}\n{Camera.main.ViewportToScreenPoint(e.GetAnchorMin())}\n{tt.position}");
            //Helpers.DebugLine($"\n{e.GetPosition() / gameScale}");
            // Original object are moved by anchors
            Vector3 cPos = Camera.main.ViewportToScreenPoint(e.GetAnchorMax());
            Vector2 ePos = e.GetPosition();

            rt.anchorMin = e.GetAnchorMin();
            rt.anchorMax = e.GetAnchorMax();
            tt.anchoredPosition = e.GetPosition() / gameScale;
          }
          else
          {
            rt.anchoredPosition = e.GetPosition();
            tt.anchoredPosition = e.GetPosition(); //rt.anchoredPosition;
          }
          rt.localScale = new Vector3(e.GetScale(), e.GetScale() * e.GetDims());
          tt.localScale = rt.localScale;
        }
      }
      catch
      {
        Helpers.DebugLine($"PositionTemplate Catch: {e.name}");
      }
    }

    public static RectTransform LocateRectTransform(Groups group, string path)
    {
      try
      {
        roots.TryGetValue(group, out Transform parent);
        // We change parent to Inventory root
        if (group == Groups.Inventory) parent = InventoryGui.instance.transform.Find("root");

        return parent.Find(path).GetComponent<RectTransform>();
      }
      catch
      {
        return null;
      }
    }

    public static RectTransform LocateTemplateRect(Groups group, string name)
    {
      try
      {
        return baseRoot.Find($"{name}{templateSuffix}").GetComponent<RectTransform>();
      }
      catch
      {
        Helpers.DebugLine($"Unable to find template for {name}", true, true);
        return null;
      }
    }

    public static void PositionTemplates()
    {
      foreach (HudElement e in elements) PositionTemplate(e);
    }

    private static void CreateTemplates()
    {
      List<HudElement> unusedElements = new List<HudElement>();
      foreach (HudElement e in elements)
      {
        try
        {
          RectTransform rt = LocateRectTransform(e.group, e.path);
          if (e.GetPosition() == Vector2.zero)
          {
            if (e.group == Groups.Inventory)
            {
              e.SetPosition(rt.anchoredPosition);
              // This elements depend on anchors, set these
              e.SetAnchors(rt.anchorMin, rt.anchorMax);
              //e.SetPosition(rt.anchorMax);
            }
            else
            {
              e.SetPosition(rt.anchoredPosition);
            }
          }
          AddTemplateToHud(e, rt);
        }
        catch
        {
          unusedElements.Add(e);
        }
      }
      // Remove unused elements from main ElementList
      if (unusedElements.Count > 0)
      {
        Helpers.DebugLine($"Removing {unusedElements.Count} unused elements.");
        foreach (HudElement e in unusedElements) elements.Remove(e);
      }
    }

    private static void AddTemplateToHud(HudElement element, RectTransform rt)
    {
      // Should we add these to their own elements? Based on their group?
      // roots.TryGetValue(Groups.HudRoot, out Transform templateRoot); // Everything on hudRoot
      // roots.TryGetValue(element.group, out Transform templateRoot);


      Transform go = UnityEngine.Object.Instantiate(hudRoot.Find("BuildHud/SelectedInfo"), baseRoot);
      go.gameObject.name = $"{element.name}{templateSuffix}";
      go.Find("selected_piece").gameObject.SetActive(false);
      go.Find("requirements").gameObject.SetActive(false);

      Text t = go.gameObject.AddComponent<Text>();
      t.text = $"{element.displayName}";
      t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
      t.fontSize = 20;
      t.alignment = TextAnchor.MiddleCenter;
      go.gameObject.SetActive(false); // Have it hidden when added

      RectTransform templateRT = go.GetComponent<RectTransform>();
      templateRT.pivot = rt.pivot;
      templateRT.anchorMin = rt.anchorMin;
      templateRT.anchorMax = rt.anchorMax;
      templateRT.offsetMin = rt.offsetMin;
      templateRT.offsetMax = rt.offsetMax;
      templateRT.sizeDelta = rt.sizeDelta;
      templateRT.anchoredPosition = rt.anchoredPosition;
      templateRT.position = rt.position;
      templateRT.localEulerAngles = rt.localEulerAngles;
      t.resizeTextForBestFit = true;
    }
  }

  [Serializable]
  public class HudElement
  {
    public string name;
    public string displayName;
    public float x;
    public float y;
    public float scale;
    public float dimensions;

    public string path;
    /// <summary>
    /// Layer Group where the element belongs
    /// </summary>
    public Groups group;
    // AnchorMin
    private float anchorMinX;
    private float anchorMinY;
    // AnchorMax
    private float anchorMaxX;
    private float anchorMaxY;


    public HudElement(string name, Vector2 position, float scale, float dimensions, string displayName, Groups group, string path)
    {
      this.name = name;
      this.displayName = displayName;
      this.x = position.x;
      this.y = position.y;
      this.scale = scale;
      this.dimensions = dimensions;
      this.path = path;
      this.group = group;
    }

    public string GetName() => name;
    public string SetName(string name) => this.name = name;
    public Vector2 GetPosition() => new Vector2(x, y);
    public void SetPosition(Vector2 pos)
    {
      x = pos.x;
      y = pos.y;
    }
    public float GetScale() => scale;
    public void SetScale(float change)
    {
      scale = (float)Math.Round(Mathf.Abs(scale + change), 1);
    }
    public float GetDims() => dimensions;
    public void SetDims(float change)
    {
      dimensions = (float)Math.Round(Mathf.Abs(dimensions + change), 2);
    }

    public void SetAnchors(Vector2 min, Vector2 max)
    {
      this.anchorMinX = min.x;
      this.anchorMinY = min.y;
      this.anchorMaxX = max.x;
      this.anchorMaxY = max.y;
    }
    public void UpdateAnchors(Vector2 min, Vector2 max)
    {
      this.anchorMinX += min.x;
      this.anchorMinY += min.y;
      this.anchorMaxX += max.x;
      this.anchorMaxY += max.y;
    }
    public Vector2 GetAnchorMin() => new Vector2(anchorMinX, anchorMinY);
    public Vector2 GetAnchorMax() => new Vector2(anchorMaxX, anchorMaxY);
  }

  public struct Element
  {
    /// <summary>
    /// Used as an unique value, unique name to the element.
    /// If no path is given, this needs to be elements path as well.
    /// </summary>
    public string name; // We see this as unique. Might cause issues later on?
    /// <summary>
    /// Use custom name on the template when user edits HUD
    /// </summary>
    public string displayName;
    /// <summary>
    /// On what layer group should the element be editable. This is as well the parent element where path is related.
    /// </summary>
    public Groups group;
    /// <summary>
    /// Path of the element. Relative to parent.
    /// </summary>
    public string locationPath;

    public Element(string name, Groups group, string locationPath = "", string displayName = "")
    {
      this.name = name;
      this.group = group;
      this.locationPath = locationPath == "" ? name : locationPath;
      this.displayName = displayName == "" ? name : displayName;
    }
  };

  public enum Groups
  {
    HudRoot,
    Inventory,
    Other // Is this enough, or should we just specify everything.
  }

  public enum ParentRoot
  {
    Hud,
    Inventory,
    HudMessage,
    TopLeftMessage,
    Chat,
    EnemyHud,
    Store,
    Menu
  }
}