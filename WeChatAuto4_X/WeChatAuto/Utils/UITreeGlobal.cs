using System;
using Microsoft.Extensions.Logging;

namespace WeChatAuto.Utils
{
    /// <summary>
    /// 记录一些异变化的UI Tree路径.
    /// </summary>
    public static class UITreeGlobal
    {
        //消息列表的根listbox
        public static string MessageRootPath = "/Group/Custom/Group/Group/Group/Custom/Custom/Custom/Group/Custom/Custom/Group/Custom/Group/Group/List[@Name='消息'][@AutomationId='chat_message_list'][@ClassName='mmui::RecyclerListView'] | /Group/Custom/Group/Group/Group/Custom/Custom/Custom/Group/Custom/Custom/Group/Custom/Group/List[@Name='消息'][@AutomationId='chat_message_list'][@ClassName='mmui::RecyclerListView'] | /Group/Group/Group/Custom/Group/Group/List[@Name='消息'][@AutomationId='chat_message_list'][@ClassName='mmui::RecyclerListView'] | /Group/Group/Group/Custom/Group/List[@Name='消息'][@AutomationId='chat_message_list'][@ClassName='mmui::RecyclerListView']";
        public static string MessagePopupMenu_Copy = "/Window[@Name='Weixin']/MenuItem[@Name='复制'][@ClassName='mmui::XMenuView'] | /Window[@Name='Weixin']/MenuItem[@Name='复制'][@ClassName='mmui::XMenu']";
        public static string MessagePopupMenu_Download = "/Window[@Name='Weixin']/MenuItem[@Name='下载'][@ClassName='mmui::XMenuView'] | /Window[@Name='Weixin']/MenuItem[@Name='下载'][@ClassName='mmui::XMenu']";
        public static string Search_Bar_Edit="/Group/Custom/Group/Group/Group/Custom/Custom/Group/Group/Group/Group/Custom/Group/Edit[@Name='搜索'] | /Group/Custom/Group/Group/Group/Custom/Custom/Group/Group/Group/Group/Group/Edit[@Name='搜索']";
        public static string Search_Bar_PopupMenu="/Window[@Name='Weixin']/Group/Group/List[@AutomationId='search_list'] | /Window[@Name='Weixin']/Group/List[@AutomationId='search_list']";
    }
}