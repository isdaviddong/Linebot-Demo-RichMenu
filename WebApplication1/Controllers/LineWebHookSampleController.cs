using isRock.LineBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class LineBotWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
         string channelAccessToken = System.Configuration.ConfigurationManager.AppSettings["ChannelAccessToken"].ToString();
        const string AdminUserId= "!!!改成你的AdminUserId!!!";

        [Route("api/LineWebHookSample")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            //設定ChannelAccessToken(或抓取Web.Config)
            this.ChannelAccessToken = channelAccessToken;
            //取得Line Event(範例，只取第一個)
            var LineEvent = this.ReceivedMessage.events.FirstOrDefault();

            try
            {
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();

                //檢查如果menu1,2選單沒有建立，則建立
                if (System.Web.HttpContext.Current.Application["menu1"] == null || System.Web.HttpContext.Current.Application["menu2"] == null)
                    CreateRichMenus();

                //取得當前用戶的預設選單編號
                var UserMenu = isRock.LineBot.Utility.GetRichMenuIdOfUser(LineEvent.source.userId, channelAccessToken);

                //如果當前用戶沒有預設選單編號
                if (UserMenu == null)
                {
                    //設為第一個
                    isRock.LineBot.Utility.LinkRichMenuToUser(
                        System.Web.HttpContext.Current.Application["menu1"].ToString(), LineEvent.source.userId, channelAccessToken);

                    UserMenu = isRock.LineBot.Utility.GetRichMenuIdOfUser(LineEvent.source.userId, channelAccessToken);
                }

                //取得當前用戶的預設選單
                var Menu = isRock.LineBot.Utility.GetRichMenu(UserMenu.richMenuId, channelAccessToken);
                //如果當前用戶沒有預設選單
                if (Menu == null)
                {
                    //設為第一個
                    isRock.LineBot.Utility.LinkRichMenuToUser(
                        System.Web.HttpContext.Current.Application["menu1"].ToString(), LineEvent.source.userId, channelAccessToken);

                    UserMenu = isRock.LineBot.Utility.GetRichMenuIdOfUser(LineEvent.source.userId, channelAccessToken);
                    Menu = isRock.LineBot.Utility.GetRichMenu(UserMenu.richMenuId, channelAccessToken);
                }

                //處理收到的訊息
                if (LineEvent.type == "message")
                {
                    if (LineEvent.message.type == "text") //收到文字
                    {
                        switch (LineEvent.message.text)
                        {
                            case "/左":
                                //如果當前選單是 menu2, 切換成menu1
                                if (Menu.chatBarText == "快捷選單B")
                                {
                                    //設為第一個
                                    isRock.LineBot.Utility.LinkRichMenuToUser(
                                        System.Web.HttpContext.Current.Application["menu1"].ToString(), LineEvent.source.userId, channelAccessToken);
                                }
                                break;
                            case "/右":
                                //如果當前選單是 menu1, 切換成menu2
                                if (Menu.chatBarText == "快捷選單A")
                                {
                                    //設為第一個
                                    isRock.LineBot.Utility.LinkRichMenuToUser(
                                        System.Web.HttpContext.Current.Application["menu2"].ToString(), LineEvent.source.userId, channelAccessToken);
                                }
                                break;
                            case "/reset":
                                //取消快捷選單
                                isRock.LineBot.Utility.UnlinkRichMenuFromUser(
                                    LineEvent.source.userId, channelAccessToken);
                                this.ReplyMessage(LineEvent.replyToken, "用戶快捷選單已取消!");
                                break;
                            default:
                                this.ReplyMessage(LineEvent.replyToken, "你說了:" + LineEvent.message.text);
                                break;
                        }
                    }
                    if (LineEvent.message.type == "sticker") //收到貼圖
                        this.ReplyMessage(LineEvent.replyToken, 1, 2);
                }
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //如果發生錯誤，傳訊息給Admin
                this.ReplyMessage(LineEvent.replyToken, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }

        public void CreateRichMenus()
        {
            //建立RuchMenu
            var item1 = new isRock.LineBot.RichMenu.RichMenuItem();
            var item2 = new isRock.LineBot.RichMenu.RichMenuItem();
            item1.name = "no name";
            item1.chatBarText = "快捷選單A";
            item1.selected = true;
            item2.name = "no name";
            item2.chatBarText = "快捷選單B";
            item2.selected = true;

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
            item1.areas.Add(leftButton);
            item1.areas.Add(rightButton);

            item2.areas.Add(leftButton);
            item2.areas.Add(rightButton);

            //建立Menu Item並綁定指定的圖片
            var menu1 = isRock.LineBot.Utility.CreateRichMenu(
                    item1, new Uri("http://arock.blob.core.windows.net/blogdata201902/test01.png"), channelAccessToken);
            var menu2 = isRock.LineBot.Utility.CreateRichMenu(
                 item2, new Uri("http://arock.blob.core.windows.net/blogdata201902/03-223328-2405ca23-08e4-404b-8df5-db625177bbd4.png"), channelAccessToken);

            System.Web.HttpContext.Current.Application["menu1"] = menu1.richMenuId;
            System.Web.HttpContext.Current.Application["menu2"] = menu2.richMenuId;
        }
    }
}
