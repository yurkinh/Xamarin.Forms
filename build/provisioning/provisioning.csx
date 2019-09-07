var channel = Env("CHANNEL") ?? "Stable";

if (IsMac)
{
  Item (XreItem.Xcode_10_3_0).XcodeSelect ();
  Item ("https://download.visualstudio.microsoft.com/download/pr/edd7782e-dc4f-4be5-9b55-8a51eeb7a718/ed8c238ec095b5d1051e2539cea38113/xamarin.android-10.0.0.40.pkg");
	
}

Console.WriteLine(channel);
XamarinChannel(channel);