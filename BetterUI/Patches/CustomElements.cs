using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.Patches
{
  static class CustomElements
  {
    public static class HealthBar
    {
      public static readonly string objectName = "BetterUI_HPBar";
      private static RectTransform healthBarRoot;
      private static GuiBar healthBarSlow;
      private static GuiBar healthBarFast;
      private static Text healthText;

      public static void Create()
      {
        try
        {
          // Hide original healthBar
          Hud.instance.transform.Find("hudroot").Find("healthpanel").gameObject.SetActive(false);

          healthBarRoot = UnityEngine.Object.Instantiate(Hud.instance.m_healthBarRoot, Hud.instance.transform.Find("hudroot"));
          healthBarRoot.gameObject.name = objectName;

          // Rotate this to 90
          int rot = 90 - (Main.healthBarRotation.Value / 90 % 4 * 90);
          healthBarRoot.localEulerAngles = new Vector3(0, 0, rot);

          healthBarFast = healthBarRoot.Find("fast").GetComponent<GuiBar>();
          healthBarSlow = healthBarRoot.Find("slow").GetComponent<GuiBar>();

          healthBarRoot.Find("fast").Find("bar").Find("HealthText").gameObject.SetActive(false);

          healthText = UnityEngine.Object.Instantiate(healthBarRoot.Find("fast").Find("bar").Find("HealthText").GetComponent<Text>(), healthBarRoot);
          healthText.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -rot);
          healthText.GetComponent<RectTransform>().localScale = new Vector3(0.65f, 0.65f, 1f);
          healthText.gameObject.SetActive(true);

          // Resize to a more "slim" rectangle - authors preference
          healthBarRoot.Find("border").GetComponent<RectTransform>().localScale = new Vector3(1f, 0.65f, 1f);
          healthBarRoot.Find("bkg").GetComponent<RectTransform>().localScale = new Vector3(1f, 0.65f, 1f);
          healthBarFast.GetComponent<RectTransform>().localScale = new Vector3(1f, 0.65f, 1f);
          healthBarSlow.GetComponent<RectTransform>().localScale = new Vector3(1f, 0.65f, 1f);

        }
        catch (Exception e)
        {
          Debug.LogError($"HealthBar.Create() {e.Message}");
        }
      }

      public static void Update(float max, float hp)
      {
        try
        {
          healthBarFast.SetMaxValue(max);
          healthBarFast.SetValue(hp);
          healthBarSlow.SetMaxValue(max);
          healthBarSlow.SetValue(hp);
          healthText.text = $"{Mathf.CeilToInt(hp)}/{Mathf.CeilToInt(max)}";
        }
        catch (Exception e)
        {
          Debug.LogError($"HealthBar.Update() {e.Message}");
        }
      }
    }

    public static class StaminaBar
    {
      public static readonly string objectName = "BetterUI_StaminaBar";
      private static RectTransform staminaBarRoot;
      private static GuiBar staminaBarSlow;
      private static GuiBar staminaBarFast;
      private static Text staminaText;

      public static void Create()
      {
        try
        {
          // Hide original staminaBar
          Hud.instance.transform.Find("hudroot").Find("staminapanel").gameObject.SetActive(false);

          staminaBarRoot = UnityEngine.Object.Instantiate(Hud.instance.m_healthBarRoot, Hud.instance.transform.Find("hudroot"));
          staminaBarRoot.gameObject.name = objectName;

          int rot = 90 - (Main.staminaBarRotation.Value / 90 % 4 * 90);
          staminaBarRoot.localEulerAngles = new Vector3(0, 0, rot);

          staminaBarFast = staminaBarRoot.Find("fast").GetComponent<GuiBar>();
          staminaBarSlow = staminaBarRoot.Find("slow").GetComponent<GuiBar>();

          staminaBarRoot.Find("fast").Find("bar").Find("HealthText").gameObject.SetActive(false);

          staminaText = UnityEngine.Object.Instantiate(staminaBarRoot.Find("fast").Find("bar").Find("HealthText").GetComponent<Text>(), staminaBarRoot);
          staminaText.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -rot);
          staminaText.GetComponent<RectTransform>().localScale = new Vector3(0.65f, 0.65f, 1f);
          staminaText.gameObject.SetActive(true);

          staminaBarRoot.Find("border").GetComponent<RectTransform>().localScale = new Vector3(1f, 0.65f, 1f);
          staminaBarRoot.Find("bkg").GetComponent<RectTransform>().localScale = new Vector3(1f, 0.65f, 1f);
          staminaBarFast.GetComponent<RectTransform>().localScale = new Vector3(1f, 0.65f, 1f);
          staminaBarSlow.GetComponent<RectTransform>().localScale = new Vector3(1f, 0.65f, 1f);

          staminaBarFast.m_originalColor = Hud.instance.m_staminaBar2Fast.m_bar.GetComponent<Image>().color;
          staminaBarFast.ResetColor();
          staminaBarSlow.m_originalColor = Hud.instance.m_staminaBar2Slow.m_bar.GetComponent<Image>().color;
          staminaBarSlow.ResetColor();

          staminaBarFast.m_smoothDrain = Hud.instance.m_staminaBar2Fast.m_smoothDrain;
          staminaBarFast.m_changeDelay = Hud.instance.m_staminaBar2Fast.m_changeDelay;
          staminaBarFast.m_smoothSpeed = Hud.instance.m_staminaBar2Fast.m_smoothSpeed;
        }
        catch (Exception e)
        {
          Debug.LogError($"StaminaBar.Create() {e.Message}");
        }
      }

      public static void Update(float max, float stamina)
      {
        try
        {
          staminaBarFast.SetValue(stamina / max);
          staminaBarSlow.SetValue(stamina / max);
          staminaText.text = $"{Mathf.CeilToInt(stamina)}/{Mathf.CeilToInt(max)}";
        }
        catch (Exception e)
        {
          Debug.LogError($"StaminaBar.Update() {e.Message}");
        }
      }
    }

    public static class FoodBar
    {
      public static readonly string objectName = "BetterUI_FoodBar";
      private static RectTransform foodPanel;
      private static RectTransform foodBarRoot;
      public static RectTransform food0;
      public static RectTransform food1;
      public static RectTransform food2;

      public static void Create()
      {
        foodPanel = UnityEngine.Object.Instantiate(Hud.instance.m_healthPanel, Hud.instance.transform.Find("hudroot"));
        foodPanel.gameObject.name = objectName;
        foodPanel.gameObject.SetActive(true);
        int rot = 90 - (Main.foodBarRotation.Value / 90 % 4 * 90);
        foodPanel.localEulerAngles = new Vector3(0, 0, rot);

        foodBarRoot = foodPanel.Find("Food").GetComponent<RectTransform>();
        food0 = foodPanel.Find("food0").GetComponent<RectTransform>();
        food1 = foodPanel.Find("food1").GetComponent<RectTransform>();
        food2 = foodPanel.Find("food2").GetComponent<RectTransform>();

        food0.localEulerAngles = new Vector3(0, 0, -rot);
        food1.localEulerAngles = new Vector3(0, 0, -rot);
        food2.localEulerAngles = new Vector3(0, 0, -rot);

        // Stuff to remove / hide
        foodPanel.Find("Health").gameObject.SetActive(false);
        foodPanel.Find("darken").gameObject.SetActive(false);
        foodPanel.Find("healthicon").gameObject.SetActive(false);
        foodPanel.Find("foodicon").gameObject.SetActive(false);
      }

      public static void Update(Player player)
      {
        List<Player.Food> foods = player.GetFoods();
        float baseHP = player.GetBaseFoodHP() / 25f * 32f;
        foodBarRoot.Find("baseBar").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, baseHP);
        float barLength = baseHP;
        for (int i = 0; i < Hud.instance.m_foodBars.Length; i++)
        {
          Image image = Hud.instance.m_foodBars[i]; // foodBarRoot (bar1, bar2, bar3, baseBar?)
          Image icon = Hud.instance.m_foodIcons[i]; // food0 -> foodicon0

          if (i < foods.Count)
          {
            Player.Food food = foods[i];
            Image tmpImage = foodBarRoot.Find(image.name).GetComponent<Image>();
            Image tmpIcon = foodPanel.Find($"food{i}").Find($"foodicon{i}").GetComponent<Image>();
            Text tmpText = foodPanel.Find($"food{i}").Find($"time").GetComponent<Text>();
            tmpImage.gameObject.SetActive(true);
            tmpImage.color = image.color;
            tmpImage.rectTransform.anchoredPosition = image.rectTransform.anchoredPosition;
            tmpImage.rectTransform.sizeDelta = image.rectTransform.sizeDelta;
            float barWidth = food.m_health / 25f * 32f;
            tmpImage.rectTransform.anchoredPosition = new Vector2(barLength, 0f);
            tmpImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Ceil(barWidth));
            barLength += barWidth;

            tmpIcon.sprite = icon.sprite;
            tmpIcon.gameObject.SetActive(true);
            tmpIcon.color = icon.color;

            tmpText.gameObject.SetActive(true);
            if (food.m_time >= 60f)
            {
              tmpText.text = Mathf.CeilToInt(food.m_time / 60f) + "m";
              tmpText.color = Color.white;
            }
            else
            {
              tmpText.text = Mathf.FloorToInt(food.m_time) + "s";
              tmpText.color = new Color(1f, 1f, 1f, 0.4f + Mathf.Sin(Time.time * 10f) * 0.6f);
            }
          }
          else
          {
            Image tmpImage = foodBarRoot.Find(image.name).GetComponent<Image>();
            Image tmpIcon = foodPanel.Find($"food{i}").Find($"foodicon{i}").GetComponent<Image>();
            Text tmpText = foodPanel.Find($"food{i}").Find($"time").GetComponent<Text>();
            tmpImage.gameObject.SetActive(false);
            tmpIcon.gameObject.SetActive(false);
            tmpText.gameObject.SetActive(false);
          }
        }
        float size = Mathf.Ceil(player.GetMaxHealth() / 25f * 32f);
        foodBarRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
      }
    }
  }
}
