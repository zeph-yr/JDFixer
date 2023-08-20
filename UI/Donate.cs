using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace JDFixer.UI
{
    internal class Donate
    {
        internal static string donate_clickable_text = "<#00000000>------------<#ff0080ff><size=85%>♡ Donate";
        internal static string donate_clickable_hint = "If you'd like to support my work";

        internal static string donate_modal_text_static_1 = "<size=85%><#ffff00ff><u>Support JDFixer</u><size=75%><#cc99ffff>\nHave you have been enjoying my creations and\nyou wish to support me?";
        internal static string donate_modal_text_static_2 = "<size=70%><#ff0080ff>With much love, ♡ Zeph<#00000000>------";

        internal static string donate_modal_text_dynamic = "";
        internal static string donate_modal_hint_dynamic = "";
        internal static string donate_update_dynamic = "";

        internal static void Refresh_Text()
        {
            if (donate_modal_text_dynamic == "")
            {
                _ = Get_Donate_Modal_Text();
            }
        }

        internal static void Patreon()
        {
            Process.Start("https://www.patreon.com/xeph_yr");
        }

        internal static void Kofi()
        {
            Process.Start("https://ko-fi.com/zeph_yr");
        }

        private static async Task Get_Donate_Modal_Text()
        {
            //Plugin.Log.Debug("reply: " + donate_modal_text_dynamic);
            string reply_text = "Loading...";
            string reply_hint = "";
            string reply_update = "";

            using (WebClient client = new WebClient())
            {
                try
                {
                    reply_text = await client.DownloadStringTaskAsync("https://www.xephai.com/jd/?a=JDFIXER&b=text");
                }
                catch
                {
                    reply_text = await client.DownloadStringTaskAsync("https://raw.githubusercontent.com/zeph-yr/Shoutouts/main/README.md");
                    Plugin.Log.Debug("Failed to fetch Donate info");
                }
                try
                {
                    reply_hint = await client.DownloadStringTaskAsync("https://www.xephai.com/jd/?a=JDFIXER&b=hint");
                }
                catch
                {
                    reply_hint = await client.DownloadStringTaskAsync("https://raw.githubusercontent.com/zeph-yr/Shoutouts/main/hoverhints.txt");
                    Plugin.Log.Debug("Failed to fetch Donate info");
                }
                try
                {
                    reply_update = await client.DownloadStringTaskAsync("https://www.xephai.com/jd/?a=JDFIXER&b=update");
                }
                catch
                {
                    reply_update = await client.DownloadStringTaskAsync("https://raw.githubusercontent.com/zeph-yr/Shoutouts/main/whatsnew.txt");
                    Plugin.Log.Debug("Failed to fetch Donate info");
                }
            }

            donate_modal_text_dynamic = reply_text;

            int hint_start = reply_hint.IndexOf("[JDFIXER]");
            int hint_end = reply_hint.IndexOf("###", hint_start);

            if (hint_start != -1)
            {
                //Plugin.Log.Debug("reply: " + reply_hint);
                //Plugin.Log.Debug("start: " + hint_start + " end: " + hint_end);
                donate_modal_hint_dynamic = reply_hint.Substring(hint_start + 9, hint_end - hint_start - 9); // Yes. And no, it's not wrong.
            }

            int update_start = reply_update.IndexOf("[JDFIXER]");
            int update_end = reply_update.IndexOf("###", update_start);
            if (update_start != -1)
            {
                donate_update_dynamic = reply_update.Substring(update_start + 9, update_end - update_start - 9);
            }
        }
    }
}