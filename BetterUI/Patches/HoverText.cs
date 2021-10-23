using UnityEngine;

namespace BetterUI.Patches
{
  static class HoverText
  {
    private static readonly string useKey = "[<color=yellow><b>$KEY_Use</b></color>]";
    private static readonly string _containerBase = "[<color=yellow><b>$KEY_Use</b></color>] $piece_container_open";
    // [E] Cook item
    private static readonly string _cookItem = "[<color=yellow><b>$KEY_Use</b></color>] $piece_cstand_cook";
    // [1-8] Cook Item
    private static readonly string _selectItem = "[<color=yellow><b>1-8</b></color>] $piece_cstand_cook";
    private static readonly string overCookColor = "red";

    private static readonly string smelterRoof = "<color=yellow>$piece_smelter_reqroof</color>";

    public static bool PatchFermenter(Fermenter fermenter, ref string hoverText)
    {
      switch (fermenter.GetStatus())
      {
        case Fermenter.Status.Fermenting:
          string contentName = fermenter.GetContentName();
          if (fermenter.m_exposed) // Why do we need to re-check? Ain't Fermenter.Status.Exposed enough? - Wack original code.
          {
            hoverText = Localization.instance.Localize(fermenter.m_name + " ( " + contentName + ", $piece_fermenter_exposed )");
            return false;
          }
          string time = Main.timeLeftStyleFermenter.Value == 1 ?
            $"{fermenter.GetFermentationTime() / fermenter.m_fermentationDuration:P0}" :
            Helpers.TimeString(fermenter.m_fermentationDuration - fermenter.GetFermentationTime());

          hoverText = Localization.instance.Localize($"{contentName}\n$piece_fermenter_fermenting: {time}");
          return false;

        case Fermenter.Status.Ready:
          string contentName2 = fermenter.GetContentName();
          hoverText = Localization.instance.Localize($"{fermenter.m_name}, $piece_fermenter_ready \n{contentName2}\n[<color=yellow><b>$KEY_Use</b></color>] $piece_fermenter_tap");
          return false;

        default:
          return true;
      }
    }

    public static bool PatchPlant(Plant plant, ref string hoverText)
    {
      switch (plant.m_status)
      {
        case Plant.Status.Healthy:
          string time = Main.timeLeftStylePlant.Value == 1 ?
            $"{plant.TimeSincePlanted() / plant.GetGrowTime():P0}" :
            Helpers.TimeString(plant.GetGrowTime() - plant.TimeSincePlanted());

          hoverText = Localization.instance.Localize($"{plant.m_name}\n{time}");
          return false;

        default:
          return true;
      }
    }

    public static string PatchContainer(Container container)
    {
      /*
      string room = Main.chestHasRoomStyle.Value == 1 ? 
        $"{container.m_inventory.SlotsUsedPercentage():F0}%" : 
        $"{container.m_inventory.NrOfItems()}/{container.m_inventory.GetWidth() * container.m_inventory.GetHeight()}";
      */
      string room;
      switch (Main.chestHasRoomStyle.Value)
      {
        case 1:
          room = $"{container.m_inventory.SlotsUsedPercentage():F0}%";
          break;
        case 2:
          room = $"{container.m_inventory.NrOfItems()}/{container.m_inventory.GetWidth() * container.m_inventory.GetHeight()}";
          break;
        case 3:
          room = $"{container.m_inventory.GetEmptySlots()}";
          break;
        default:
          room = $"{container.m_inventory.SlotsUsedPercentage():F0}%";
          break;
      }
      return Localization.instance.Localize($"{container.m_name} ( {room} )\n{_containerBase}");
    }

    public static bool PatchCookingStation(CookingStation cookingStation, ref string hoverText)
    {
      if (cookingStation.m_nview.IsOwner() && cookingStation.IsFireLit())
      {
        string cookingItems = "";
        int items = 0;

        for (int i = 0; i < cookingStation.m_slots.Length; i++)
        {
          cookingStation.GetSlot(i, out string text, out float num, out CookingStation.Status status);
          if (text != "" && text != cookingStation.m_overCookedItem.name)
          {
            CookingStation.ItemConversion itemConversion = cookingStation.GetItemConversion(text);
            if (text != null)
            {
              items++;
              if (num > itemConversion.m_cookTime) // Item overCooking
              {
                string time = Main.timeLeftStyleCookingStation.Value == 1 ? $"{num / (itemConversion.m_cookTime * 2f):P0}" : Helpers.TimeString(itemConversion.m_cookTime * 2f - num);
                cookingItems += $"\n{cookingStation.m_overCookedItem.GetHoverName()}: <color={overCookColor}>{time}</color>";
              }
              else
              {
                string time = Main.timeLeftStyleCookingStation.Value == 1 ? $"{num / itemConversion.m_cookTime:P0}" : Helpers.TimeString(itemConversion.m_cookTime - num);
                cookingItems += $"\n{itemConversion.m_to.GetHoverName()}: {time}";
              }
            }
          }
        }
        if (items > 0)
        {
          hoverText = items >= cookingStation.m_slots.Length ?
            Localization.instance.Localize($"{cookingStation.m_name}{cookingItems}") :
            Localization.instance.Localize($"{cookingStation.m_name}\n{_cookItem}\n{_selectItem}{cookingItems}");
          return false; // Overwrite games default string
        }
      }
      return true;
    }

    public static void PatchSmelter(Smelter smelter)
    {
      if (smelter.m_emptyOreSwitch && smelter.m_spawnStack)
      {
        int processedQueueSize = smelter.GetProcessedQueueSize();
        smelter.m_emptyOreSwitch.m_hoverText = $"{smelter.m_name} {processedQueueSize} $piece_smelter_ready \n{useKey} {smelter.m_emptyOreTooltip}";
      }
      int queueSize = smelter.GetQueueSize();
      smelter.m_addOreSwitch.m_hoverText = $"{smelter.m_name} ({queueSize}/{smelter.m_maxOre}) ";

      if (queueSize > 0) // This codeline is run every tick when windmill is on!!
      {
        Debug.Log($"{smelter.GetBakeTimer()}, {smelter.m_secPerProduct}, {queueSize}");
  
        smelter.m_addOreSwitch.m_hoverText += $"{Helpers.TimeString(smelter.m_secPerProduct * queueSize - smelter.GetBakeTimer())}";
        // 8sec - 10sec (30sec)
        // 9sec - 10sec (30sec)
      }

      if (smelter.m_requiresRoof && !smelter.m_haveRoof && Mathf.Sin(Time.time * 10f) > 0f)
      {
        Switch addOreSwitch = smelter.m_addOreSwitch;
        addOreSwitch.m_hoverText += $" {smelterRoof}";
      }
      Switch addOreSwitch2 = smelter.m_addOreSwitch;
      addOreSwitch2.m_hoverText = $"{addOreSwitch2.m_hoverText} \n{useKey} {smelter.m_addOreTooltip}";
    }

    private static void CalculateSmelterBakeTime(Smelter smelter)
    {
      double deltaTime = smelter.GetDeltaTime();
      float accumulator = smelter.GetAccumulator();
      accumulator += (float)deltaTime;
      float power = smelter.m_windmill ? smelter.m_windmill.GetPowerOutput() : 1f;

      while (accumulator >= 1f)
      {
        accumulator -= 1f;
        float fuel = smelter.GetFuel();
        string queuedOre = smelter.GetQueuedOre();
        if ((smelter.m_maxFuel == 0 || fuel > 0f) && queuedOre != "" && smelter.m_secPerProduct > 0f && (!smelter.m_requiresRoof || smelter.m_haveRoof))
        {
          float speed = 1f * power;
          if (smelter.m_maxFuel > 0)
          {
            float usage = smelter.m_secPerProduct / (float)smelter.m_fuelPerProduct;
            fuel -= speed / usage;
            if (fuel < 0f) fuel = 0f;
            smelter.SetFuel(fuel);
          }
          float bakeTime = smelter.GetBakeTimer();
          bakeTime += speed;
          smelter.SetBakeTimer(bakeTime);
          if (bakeTime > smelter.m_secPerProduct)
          {
            smelter.SetBakeTimer(0f);
            smelter.RemoveOneOre();
            smelter.QueueProcessed(queuedOre);
          }
        }
      }

      if (smelter.GetQueuedOre() == "" || ((float)smelter.m_maxFuel > 0f && smelter.GetFuel() == 0f))
      {
        smelter.SpawnProcessed();
      }

      smelter.SetAccumulator(accumulator);
    }
  }
}