namespace EditorShortCuts.Utils
{
    public class Tuple<TItem1, TItem2>
    {
        public TItem1 Item1 { get; set; }
        public TItem2 Item2 { get; set; }

        public Tuple()
        {
            Item1 = default;
            Item2 = default;
        }

        public Tuple(TItem1 Item1, TItem2 Item2)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
        }
    }
}
