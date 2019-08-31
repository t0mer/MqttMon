using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.IO;


namespace MqttMon
{
    public partial class MqttMon : Form
    {
        private MqttClient client;
        public MqttMon()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


        }

        private void Connect()
        {
            try
            {
                if (client == null)
                {
                    this.output.AppendLine(Color.Yellow, "Connecting To server \n");
                    client = new MqttClient(this.address.Text);
                    client.MqttMsgPublished += client_MqttMsgPublished;
                    client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
                    client.ConnectionClosed += Client_ConnectionClosed;
                    var resule = client.Connect(new Guid().ToString(), this.Username.Text, this.Password.Text);
                    ushort msgId = client.Subscribe(new string[] { "#", "w" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    this.output.AppendLine(Color.Green, "Connected To server \n");
                }
            }
            catch (Exception ex)
            {
                this.output.AppendLine(System.Drawing.Color.Red, ex.Message.ToString() + "\n");
                this.output.AppendLine(System.Drawing.Color.Red, "unable to connect to the server, please check your connection settings \n");
                client = null;

            }
        }

        private void DisConnect()
        {
            try
            {
                if (client != null)
                {
                    if (client.IsConnected)
                    {
                        this.output.AppendLine(Color.Yellow, "Disonnecting from server \n");
                        client.Unsubscribe(new string[] { "#", "w" });
                        client.MqttMsgPublished -= client_MqttMsgPublished;
                        client.MqttMsgPublishReceived -= Client_MqttMsgPublishReceived;
                        client.ConnectionClosed -= Client_ConnectionClosed;
                        client.Disconnect();
                        this.output.AppendLine(Color.Yellow, "Disconnected from server \n");

                    }
                }
            }
            catch (Exception ex)
            {
                this.output.AppendLine(System.Drawing.Color.Red, ex.Message.ToString());
            }
        }



        private void Client_ConnectionClosed(object sender, EventArgs e)
        {

        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            this.output.AppendLine(Color.Green, "Topic Received: " + e.Topic + "\n");
            this.output.AppendLine(Color.White, "Data Received: " + Encoding.Default.GetString(e.Message) + "\n");
            this.output.AppendLine(Color.Green, "\n");
        }

        private void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.address.Text.Trim())){
                this.output.AppendLine(Color.Red, "Please enter Server Address \n");
                this.address.Focus();
                return;
            }

            //if(!ValidateIPv4(this.address.Text.Trim()))
            //{
            //    this.output.AppendLine(Color.Red, "Please enter a valid Server Address \n");
            //    this.address.Focus();
            //    return;
            //}

            if (string.IsNullOrEmpty(this.Username.Text.Trim())){
                this.output.AppendLine(Color.Red, "Please enter Username \n");
                this.Username.Focus();
                return;
            }


            if (string.IsNullOrEmpty(this.Password.Text.Trim())){
                this.output.AppendLine(Color.Red, "Please enter Password \n");
                this.Password.Focus();
                return;
            }
            this.Connect();
            this.CheckState();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            this.DisConnect();
            this.CheckState();
        }

        private void CheckState()
        {

            if (client.IsConnected)
            {
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

            }
            else
            {
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                client = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DisConnect();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            this.output.Clear();
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
           
        }

    }
}
