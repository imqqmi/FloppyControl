namespace FloppyControlApp
{
	public partial class FloppyControl
	{
		#region model classes

		public class ComboboxItem
        {
            public string Text { get; set; }
            public int Id { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
