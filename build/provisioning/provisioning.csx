var channel = Env("CHANNEL") ?? "Stable";

Console.WriteLine(channel);

if (IsMac)
{
  Item (XreItem.Xcode_10_3_0).XcodeSelect ();
}  

XamarinChannel(channel);