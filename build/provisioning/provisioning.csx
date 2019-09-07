var channel = Env("CHANNEL") ?? "Stable";

if (IsMac)
{
  Item (XreItem.Xcode_10_3_0).XcodeSelect ();
}
Console.WriteLine(channel);
XamarinChannel(channel);
Item ("https://aka.ms/xamarin-android-commercial-d16-3-macos");
	