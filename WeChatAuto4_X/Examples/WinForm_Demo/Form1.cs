using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeChatAuto.Services;
using WeChatAuto.Components;
using Microsoft.Extensions.DependencyInjection;

namespace WinForm_Demo
{
    public partial class Form1 : Form
    {
        private string wxClientName = "Alex";   //这里修改成自己的，如果想控制多个微信，这里不需要写死
        private string wxGroupName = "人工智能自动化技术讨论群";  //这里修改成自己的
        private WeChatClient client;   //微信客户端对象,如果想控制多个微信，这里不需要写死，可以保存一个数组或者字典.
        private WeChatClientFactory factory;   //通过这里随时可以获取到微信客户端对象
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var _serviceProvider = WeAutomation.Initialize(options =>
                {
                    options.DebugMode = false;
                    options.EnableOCR = true;
                });
                factory = _serviceProvider.GetRequiredService<WeChatClientFactory>();
                client = factory.GetWeChatClient(wxClientName);
            }catch(Exception ex)
            {
                MessageBox.Show("""
                    初始化微信客户端失败，请按下面步骤检查:
                    1. 是否微信开放UI Tree.
                    2. 微信是否拉到任务栏.
                    3. 是否将models文件拷贝到应用的根目录.
                    ""","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private async void button1_Click(object sender, EventArgs e)  //这里设置为async
        {
            //这里设定你自己的群
            var list = await client.GetChatGroupMemberList(wxGroupName);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    listBox1.Items.Add(item);
                }
            }
            else
            {
                listBox1.Items.Clear();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (factory != null)
            {
                factory.Dispose();    //这里最好手动释放一下资源，避免微信客户端进程残留
            }
        }
    }
}
