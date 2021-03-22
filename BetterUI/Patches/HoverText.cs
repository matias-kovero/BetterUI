namespace BetterUI.Patches
{
  static class HoverText
  {
    private static readonly string _containerBase = "[<color=yellow><b>$KEY_Use</b></color>] $piece_container_open";

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
      string room = Main.chestHasRoomStyle.Value == 1 ? 
        $"{container.m_inventory.SlotsUsedPercentage():F0}%" : 
        $"{container.m_inventory.NrOfItems()}/{container.m_inventory.GetWidth() * container.m_inventory.GetHeight()}";

      return Localization.instance.Localize($"{container.m_name} ( {room} )\n{_containerBase}");
    }
  }
}