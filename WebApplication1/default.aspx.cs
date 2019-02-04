using isRock.LineBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class _default : System.Web.UI.Page
    {
         string channelAccessToken = System.Configuration.ConfigurationManager.AppSettings["ChannelAccessToken"].ToString();
        const string AdminUserId= "!!!改成你的AdminUserId!!!";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //建立RuchMenu
            var item = new isRock.LineBot.RichMenu.RichMenuItem();
            item.name = "no name";
            item.chatBarText = "快捷選單";
            //建立左方按鈕區塊
            var leftButton = new isRock.LineBot.RichMenu.Area();
            leftButton.bounds.x = 0;
            leftButton.bounds.y = 0;
            leftButton.bounds.width = 460;
            leftButton.bounds.height = 1686;
            leftButton.action = new MessageAction() { label = "左", text = "/左" };
            //建立右方按鈕區塊
            var rightButton = new isRock.LineBot.RichMenu.Area();
            rightButton.bounds.x = 2040;
            rightButton.bounds.y = 0;
            rightButton.bounds.width = 2040 + 460;
            rightButton.bounds.height = 1686;
            rightButton.action = new MessageAction() { label = "右", text = "/右" };
            //將area加入RichMenuItem
            item.areas.Add(leftButton);
            item.areas.Add(rightButton);
            //建立Menu Item並綁定指定的圖片
            var menu = isRock.LineBot.Utility.CreateRichMenu(
                    item, new Uri("http://arock.blob.core.windows.net/blogdata201902/test01.png"), channelAccessToken);
            //將Menu Item設為預設Menu
            isRock.LineBot.Utility.SetDefaultRichMenu(menu.richMenuId, channelAccessToken);
            Response.Write($"OK, {menu.richMenuId}");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var list = isRock.LineBot.Utility.GetRichMenuList(channelAccessToken);
            foreach (var item in list.richmenus)
            {
                Response.Write($"id : { item.richMenuId}");
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            isRock.LineBot.Utility.CancelDefaultRichMenu(channelAccessToken);
            Response.Write("done!");
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            var ret=isRock.LineBot.Utility.GetRichMenuList(channelAccessToken);
            foreach (var item in ret.richmenus)
            {
                Response.Write($"<br/> {item.chatBarText}  -  {item.richMenuId}");
            }
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            var ret = isRock.LineBot.Utility.GetRichMenuList(channelAccessToken);
            foreach (var item in ret.richmenus)
            {
                isRock.LineBot.Utility.DeleteRichMenu(item.richMenuId, channelAccessToken);
               Response.Write($"<br/> {item.chatBarText}  -  {item.richMenuId} ... 已刪除");
            }
        }
    }
}