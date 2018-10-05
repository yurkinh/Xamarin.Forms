namespace Xamarin.Forms.Platform.UWP
{
	internal class ItemTemplatePair
	{
		public ItemTemplatePair(DataTemplate formsDataTemplate, object item)
		{
			FormsDataTemplate = formsDataTemplate;
			Item = item;
		}

		public DataTemplate FormsDataTemplate { get; }
		public object Item { get; }

		protected bool Equals(ItemTemplatePair other)
		{
			return Equals(FormsDataTemplate, other.FormsDataTemplate) && Equals(Item, other.Item);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != this.GetType())
			{
				return false;
			}

			return Equals((ItemTemplatePair)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((FormsDataTemplate != null ? FormsDataTemplate.GetHashCode() : 0) * 397) ^ (Item != null ? Item.GetHashCode() : 0);
			}
		}
	}
}