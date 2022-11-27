using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace JDFixer.UI
{
    internal class Donate
    {
        internal static string donate_clickable_text = "<#00000000>------------<#ff0080ff><size=85%>♡ Donate";
        internal static string donate_clickable_hint = "If JDFixer has helped you, learn how you can help support the project";

        internal static string donate_modal_text_static_1 = "<size=85%><#ffff00ff><u>Support JDFixer</u><size=75%><#cc99ffff>\nHave you have been enjoying my creations and wish to support me?";
        internal static string donate_modal_text_static_2 = "<size=70%><#ff0080ff>With much love,<#00000000>--<#ff0080ff>\n♡ Zeph<#00000000>--";

        internal static string donate_modal_text_dynamic = "";

        internal static void Refresh_Text()
        {
            if (donate_modal_text_dynamic == "")
            {
                _ = Get_Donate_Modal_Text();
            }
        }

        internal static string Open_Donate_Browser()
        {
            Process.Start("https://www.patreon.com/xeph_yr");
            return "";
        }

        private static async Task Get_Donate_Modal_Text()
        {
            //Logger.log.Debug("reply: " + donate_modal_text_dynamic);
            string reply = "Loading...";

            using (WebClient client = new WebClient())
            {
                try
                {
                    reply = await client.DownloadStringTaskAsync("https://raw.githubusercontent.com/zeph-yr/Shoutouts/main/README.md");
                }
                catch
                {
                    reply = "Loading failed. Pls ping Zeph on Discord, TY!";
                    Logger.log.Debug("Failed to fetch Donate info");
                }
            }

            //Logger.log.Debug("reply: " + reply);
            donate_modal_text_dynamic = reply;
        }
    }
}