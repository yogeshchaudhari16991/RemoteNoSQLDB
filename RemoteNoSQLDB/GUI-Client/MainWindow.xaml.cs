/////////////////////////////////////////////////////////////////////////
// MainWindows.xaml.cs - CommService GUI Client                        //
// Ver 1.0                                                             //
// Application: Demonstration for CSE681-SMA, Project#2                //
// Language:    C#, ver 6.0, Visual Studio 2015                        //
// Platform:    Dell Inspiron 14 5000Series, Core-i5, Windows 10       //
// Author:      Yogesh Chaudhari, Student, Syracuse University         //
//              (315) 4809210, ychaudha@syr.edu                        //
/////////////////////////////////////////////////////////////////////////
/*
 *   Module Operations
 *   -----------------
 *   This module demonstrates communication between WPF Client and 
 *   Server using sender and receiver of communication channel.
 *   WPF Client can change the database state by making requests to the server
 *   WPF Client can also make queries to the database for data retrieval
 *   WPF Client reads user imput from GUI to generate messages to be sent to server
 *   It also displays write clients processing time, read client latency and server's
 *   throuput time in microseconds
 *
 * Additions to C# WPF Wizard generated code:
 * - Added reference to Communication Channel
 * - Added using Project4Starter
 *
 * Note:
 * - This client receives and sends messages.
 */
/*
 */
/*
 *   Reference:
 *   ----------
 *   Dr. Fawcett's CommPrototype Code
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 19 November 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Project4Starter;
using System.Collections;

namespace WpfApplication1
{
  public partial class MainWindow : Window
  {
    static bool firstConnect = true;
    static Receiver rcvr = null;
    static wpfSender sndr = null;
    string localAddress = "localhost";
    string localPort = "8081";
    string remoteAddress = "localhost";
    string remotePort = "8080";
    static bool flag = false; //for validation of input purpose

    /////////////////////////////////////////////////////////////////////
    // nested class wpfSender used to override Sender message handling
    // - routes messages to status textbox
    public class wpfSender : Sender
    {
      TextBox lStat_ = null;  // reference to UIs local status textbox
      System.Windows.Threading.Dispatcher dispatcher_ = null;

      public wpfSender(TextBox lStat, System.Windows.Threading.Dispatcher dispatcher)
      {
        dispatcher_ = dispatcher;  // use to send results action to main UI thread
        lStat_ = lStat;
      }
      public override void sendMsgNotify(string msg)
      {
        Action act = () => { lStat_.Text = msg; };
        dispatcher_.Invoke(act);

      }
      public override void sendExceptionNotify(Exception ex, string msg = "")
      {
        Action act = () => { lStat_.Text = ex.Message; };
        dispatcher_.Invoke(act);
      }
      public override void sendAttemptNotify(int attemptNumber)
      {
        Action act = null;
        act = () => { lStat_.Text = String.Format("attempt to send #{0}", attemptNumber); };
        dispatcher_.Invoke(act);
      }
    }
    public MainWindow()
    {
      InitializeComponent();
      lAddr.Text = localAddress;
      lPort.Text = localPort;
      rAddr.Text = remoteAddress;
      rPort.Text = remotePort;
      Title = "Prototype WPF Client: " + localPort;
      send.IsEnabled = false;
    }
    //----< trim off leading and trailing white space >------------------

    string trim(string msg)
    {
      StringBuilder sb = new StringBuilder(msg);
      for (int i = 0; i < sb.Length; ++i)
        if (sb[i] == '\n')
          sb.Remove(i, 1);
      return sb.ToString().Trim();
    }
    //----< indirectly used by child receive thread to post results >----
    //----< display received messages in GUI >---------------------------
    public void postRcvMsg(string content)
    {
      if (content.StartsWith("test-result"))
      {
        TextBlock item = new TextBlock();
        item.Text = trim(content);
        item.FontSize = 13;
        rcvmsgs.Items.Insert(0, item);
      }
      else
      {
        TextBlock item = new TextBlock();
        item.Text = trim(content);
        item.FontSize = 13;
        Result_Box.Items.Insert(0, item);
      }
    }
    //----< used by main thread >----------------------------------------

    public void postSndMsg(string content)
    {
      TextBlock item = new TextBlock();
      item.Text = trim(content);
      item.FontSize = 13;
      sndmsgs.Items.Insert(0, item);
    }
    //----< get Receiver and Sender running >----------------------------

    void setupChannel()
    {
      rcvr = new Receiver(localPort, localAddress);
      Action serviceAction = () =>
      {
        try
        {
          Message rmsg = null;
          while (true)
          {
            rmsg = rcvr.getMessage();
            Action act = () => { postRcvMsg(rmsg.content); };
            Dispatcher.Invoke(act, System.Windows.Threading.DispatcherPriority.Background);
          }
        }
        catch (Exception ex)
        {
          Action act = () => { lStat.Text = ex.Message; };
          Dispatcher.Invoke(act);
        }
      };
      if (rcvr.StartService())
      {
        rcvr.doService(serviceAction);
      }

      sndr = new wpfSender(lStat, this.Dispatcher);
    }
    //----< set up channel after entering ports and addresses >----------

    private void start_Click(object sender, RoutedEventArgs e)
    {
      localPort = lPort.Text;
      localAddress = lAddr.Text;
      remoteAddress = rAddr.Text;
      remotePort = rPort.Text;

      if (firstConnect)
      {
        firstConnect = false;
        if (rcvr != null)
          rcvr.shutDown();
        setupChannel();
      }
      rStat.Text = "connect setup";
      send.IsEnabled = true;
      connect.IsEnabled = false;
      lPort.IsEnabled = false;
      lAddr.IsEnabled = false;
    }
    //----< send a test performance request message >--------------------

    private void send_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        #region
        /////////////////////////////////////////////////////
        // This commented code was put here to allow
        // user to change local port and address after
        // the channel was started.  
        //
        // It does what is intended, but would throw 
        // if the new port is assigned a slot that
        // is in use or has been used since the
        // TCP tables were last updated.
        //
        // if (!localPort.Equals(lPort.Text))
        // {
        //   localAddress = rcvr.address = lAddr.Text;
        //   localPort = rcvr.port = lPort.Text;
        //   rcvr.shutDown();
        //   setupChannel();
        // }
        #endregion
        if (!remoteAddress.Equals(rAddr.Text) || !remotePort.Equals(rPort.Text))
        {
          remoteAddress = rAddr.Text;
          remotePort = rPort.Text;
        }
        // - Make a demo message to send
        // - You will need to change MessageMaker.makeMessage
        //   to make messages appropriate for your application design
        // - You might include a message maker tab on the UI
        //   to do this.
        MessageMaker maker = new MessageMaker();
        Message msg = maker.makeMessage(Utilities.makeUrl(lAddr.Text, lPort.Text), Utilities.makeUrl(rAddr.Text, rPort.Text));
        lStat.Text = "sending to" + msg.toUrl;
        sndr.localUrl = msg.fromUrl;
        sndr.remoteUrl = msg.toUrl;
        lStat.Text = "attempting to connect";
        if (sndr.sendMessage(msg))
          lStat.Text = "connected";
        else
          lStat.Text = "connect failed";
        postSndMsg(msg.content);
      }
      catch (Exception ex)
      {
        lStat.Text = ex.Message;
      }
    }
    //---------< chnage GUI buttons and user input fields according to
    //---------  user query selection >----------------------------------
    private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var comboBox = sender as ComboBox;

      // ... Set SelectedItem as Window Title.
      string value = comboBox.SelectedItem as string;
      clear_query_ui();
      if (value == "Insert Element")
      {
        insert_Element_UI();
      }
      if (value == "Edit Element Metadata" || value == "Edit Element Metadata and Add Children" || value == "Edit Element Metadata and Remove Children" || value == "Edit Element Metadata and Edit Payload")
      {
        edit_Element_UI();
      }
      if (value == "Delete Element")
      {
        delete_Element_UI();
      }
      if (value == "Restore Database")
      {
        restore_DB_UI();
      }
      if (value == "Persist Database")
      {
        persist_DB_UI();
      }
      if (value == "Pattern Matching in Key")
      {
        pattern_Matching_UI();
      }
      if (value == "String in Metadata")
      {
        string_Metadata_UI();
      }
      if (value == "Search Key-Value")
      {
        search_Key_Value_UI();
      }
      if (value == "Search Children")
      {
        search_Children_UI();
      }
      if (value == "Time-Date Interval")
      {
        time_Date_Interval_UI();
      }
    }

    //---------< Code to Manage query combox contents
    //---------  according to selected database >---------------
    private void comboBox_db_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      switch (comboBox_db.SelectedValue.ToString())
      {
        case "Database<string,List<string>>":
          populate_enum_db_query_comboBox();
          Number_Of_Children.Text = "Children (string,string)";
          break;
        case "Database<int,string>":
          populate_db_query_combobox();
          Number_Of_Children.Text = "Children (int,int)";
          break;
      }
    }
    private void comboBox_Loaded(object sender, RoutedEventArgs e)
    {
      // ... A List.
      List<string> data = new List<string>();
      data.Add("Insert Element");
      data.Add("Edit Element Metadata");
      data.Add("Edit Element Metadata and Add Children");
      data.Add("Edit Element Metadata and Remove Children");
      data.Add("Edit Element Metadata and Edit Payload");
      data.Add("Delete Element");
      data.Add("Persist Database");
      data.Add("Restore Database");
      data.Add("Search Key-Value");
      data.Add("Search Children");
      data.Add("Pattern Matching in Key");
      data.Add("String in Metadata");
      data.Add("Time-Date Interval");
      //data.Add("");

      // ... Get the ComboBox reference.
      var comboBox = sender as ComboBox;

      // ... Assign the ItemsSource to the List.
      comboBox.ItemsSource = data;

      // ... Make the first item selected.
      comboBox.SelectedIndex = 0;
    }
    private void comboBox_db_Loaded(object sender, RoutedEventArgs e)
    {
      List<string> data = new List<string>();
      data.Add("Database<int,string>");
      data.Add("Database<string,List<string>>");

      // ... Assign the ItemsSource to the List.
      comboBox_db.ItemsSource = data;

      // ... Make the first item selected.
      comboBox_db.SelectedIndex = 1;
    }

    private void populate_db_query_combobox()
    {
      // ... A List.
      List<string> data = new List<string>();
      data.Add("Insert Element");
      data.Add("Edit Element Metadata");
      data.Add("Delete Element");
      data.Add("Persist Database");
      data.Add("Restore Database");
      // ... Assign the ItemsSource to the List.
      comboBox_query.ItemsSource = data;
      // ... Make the first item selected.
      comboBox_query.SelectedIndex = 0;
    }

    private void populate_enum_db_query_comboBox()
    {
      // ... A List.
      List<string> data = new List<string>();
      data.Add("Insert Element");
      data.Add("Edit Element Metadata");
      data.Add("Edit Element Metadata and Add Children");
      data.Add("Edit Element Metadata and Remove Children");
      data.Add("Edit Element Metadata and Edit Payload");
      data.Add("Delete Element");
      data.Add("Persist Database");
      data.Add("Restore Database");
      data.Add("Search Key-Value");
      data.Add("Search Children");
      data.Add("Pattern Matching in Key");
      data.Add("String in Metadata");
      data.Add("Time-Date Interval");
      //data.Add("");

      // ... Assign the ItemsSource to the List.
      comboBox_query.ItemsSource = data;

      // ... Make the first item selected.
      comboBox_query.SelectedIndex = 0;
    }
    private void time_Date_Interval_UI()
    {
      S_Date.Visibility = Visibility.Visible;
      S_Date_Value.Visibility = Visibility.Visible;
      E_Date.Visibility = Visibility.Visible;
      E_Date_Value.Visibility = Visibility.Visible;
    }
    //---------< Code to Manage UI according to selected query >------
    private void string_Metadata_UI()
    {
      Pattern.Visibility = System.Windows.Visibility.Visible;
      Pattern_Value.Visibility = System.Windows.Visibility.Visible;
      Make_Query.Content = "Find String";
    }
    //---------< Code to Manage UI according to selected query >------
    private void search_Children_UI()
    {
      Key.Visibility = System.Windows.Visibility.Visible;
      Key_Value.Visibility = System.Windows.Visibility.Visible;
      Make_Query.Content = "Search Children";
    }
    //---------< Code to Manage UI according to selected query >------
    private void search_Key_Value_UI()
    {
      Key.Visibility = System.Windows.Visibility.Visible;
      Key_Value.Visibility = System.Windows.Visibility.Visible;
      Make_Query.Content = "Search Key";
    }
    //---------< Code to Manage UI according to selected query >------
    private void pattern_Matching_UI()
    {
      Pattern.Visibility = System.Windows.Visibility.Visible;
      Pattern_Value.Visibility = System.Windows.Visibility.Visible;
      Make_Query.Content = "Match Pattern";
    }
    //---------< Code to Manage UI according to selected query >------
    private void persist_DB_UI()
    {
      File_Name.Visibility = Visibility.Visible;
      File_Name_Value.Visibility = Visibility.Visible;
      Make_Query.Content = "Persist Database";
    }
    //---------< Code to Manage UI according to selected query >------
    private void restore_DB_UI()
    {
      File_Name.Visibility = Visibility.Visible;
      File_Name_Value.Visibility = Visibility.Visible;
      Make_Query.Content = "Restore Database";
    }
    //---------< Code to Manage UI according to selected query >------
    private void delete_Element_UI()
    {
      Key.Visibility = Visibility.Visible;
      Key_Value.Visibility = Visibility.Visible;
      Make_Query.Content = "Delete Element";
    }
    //---------< Code to Manage UI according to selected query >------
    private void edit_Element_UI()
    {
      Make_Query.Content = "Edit Element";
      showMetadata_UI();
    }
    //---------< Code to Manage UI according to selected query >------
    private void showMetadata_UI()
    {
      Key.Visibility = System.Windows.Visibility.Visible;
      Key_Value.Visibility = System.Windows.Visibility.Visible;
      Name.Visibility = System.Windows.Visibility.Visible;
      Name_Value.Visibility = System.Windows.Visibility.Visible;
      Descr.Visibility = System.Windows.Visibility.Visible;
      Descr_Value.Visibility = System.Windows.Visibility.Visible;
      Timestamp.Visibility = System.Windows.Visibility.Visible;
      Timestamp_Value.Visibility = System.Windows.Visibility.Visible;
      Children.Visibility = System.Windows.Visibility.Visible;
      Number_Of_Children.Visibility = System.Windows.Visibility.Visible;
      Payload.Visibility = System.Windows.Visibility.Visible;
      Number_of_Payload.Visibility = System.Windows.Visibility.Visible;
    }
    //---------< Code to Manage UI according to selected query >------
    private void insert_Element_UI()
    {
      Make_Query.Content = "Insert Element";
      showMetadata_UI();
    }
    //---------< Code to clear UI for selected query >------
    public void clear_query_ui()
    {
      Key.Visibility = System.Windows.Visibility.Hidden;
      Key_Value.Visibility = System.Windows.Visibility.Hidden;
      Name.Visibility = System.Windows.Visibility.Hidden;
      Name_Value.Visibility = System.Windows.Visibility.Hidden;
      Descr.Visibility = System.Windows.Visibility.Hidden;
      Descr_Value.Visibility = System.Windows.Visibility.Hidden;
      Timestamp.Visibility = System.Windows.Visibility.Hidden;
      Timestamp_Value.Visibility = System.Windows.Visibility.Hidden;
      Children.Visibility = System.Windows.Visibility.Hidden;
      Number_Of_Children.Visibility = System.Windows.Visibility.Hidden;
      Pattern.Visibility = System.Windows.Visibility.Hidden;
      Pattern_Value.Visibility = System.Windows.Visibility.Hidden;
      Payload.Visibility = System.Windows.Visibility.Hidden;
      Number_of_Payload.Visibility = System.Windows.Visibility.Hidden;
      File_Name.Visibility = Visibility.Hidden;
      File_Name_Value.Visibility = Visibility.Hidden;
      S_Date.Visibility = Visibility.Hidden;
      S_Date_Value.Visibility = Visibility.Hidden;
      E_Date.Visibility = Visibility.Hidden;
      E_Date_Value.Visibility = Visibility.Hidden;
    }

    //-------< on button click parse user input to generate
    //-------  messages to be sent to server >-------------------
    private void button_Click(object sender, RoutedEventArgs e)
    {
      MessageMaker maker = new MessageMaker();
      Message msg = maker.makeMessage(
        Utilities.makeUrl(lAddr.Text, lPort.Text),
        Utilities.makeUrl(rAddr.Text, rPort.Text)
      );
      if (sndr != null)
      {
        sndr.localUrl = msg.fromUrl;
        sndr.remoteUrl = msg.toUrl;
        get_DB_type(ref msg);
        msg.content += "query,querytype,";
        if (comboBox_query.SelectedItem.ToString() == "Insert Element" ||
          comboBox_query.SelectedItem.ToString() == "Edit Element Metadata" ||
          comboBox_query.SelectedItem.ToString() == "Edit Element Metadata and Add Children" ||
          comboBox_query.SelectedItem.ToString() == "Edit Element Metadata and Remove Children" ||
          comboBox_query.SelectedItem.ToString() == "Edit Element Metadata and Edit Payload" ||
          comboBox_query.SelectedItem.ToString() == "Delete Element" ||
          comboBox_query.SelectedItem.ToString() == "Persist Database" ||
          comboBox_query.SelectedItem.ToString() == "Restore Database")
        {
          write_client_query(ref msg);
        }
        else
        {
          read_client_query(ref msg);
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "First Connect to Server in Connect Tab");
      }
    }
    //---------< add database type to message >-------------
    private void get_DB_type(ref Message msg)
    {
      switch (comboBox_db.SelectedValue.ToString())
      {
        case "Database<int,string>":
          msg.content = "keytype,integer,payloadtype,String,";
          flag = false;
          break;
        case "Database<string,List<string>>":
          msg.content = "keytype,String,payloadtype,List`1,";
          flag = true;
          break;
      }
    }
    
    //---------< add querytype to message >-------------
    private void read_client_query(ref Message msg)
    {
      switch (comboBox_query.SelectedItem.ToString())
      {
        case "Search Key-Value":
          msg.content += "Search Key-Value,";
          search_Key_Value(ref msg);
          break;
        case "Search Children":
          msg.content += "Search Children,";
          search_Children(ref msg);
          break;
        case "Pattern Matching in Key":
          msg.content += "Pattern Matching,";
          pattern_Matching(ref msg);
          break;
        case "String in Metadata":
          msg.content += "String in Metadata,";
          string_Metadata(ref msg);
          break;
        case "Time-Date Interval":
          msg.content += "Time-Date Interval,";
          time_Date_Interval(ref msg);
          break;
      }
    }
    //---------< add querytype to message >-------------
    private void write_client_query(ref Message msg)
    {
      switch (comboBox_query.SelectedItem.ToString())
      {
        case "Insert Element":
          msg.content += "Insert Element,";
          insert_Element_clicked(ref msg);
          break;
        case "Edit Element Metadata":
          msg.content += "Edit Element Metadata,";
          edit_Element_Metadata_clicked(ref msg);
          break;
        case "Edit Element Metadata and Add Children":
          msg.content += "Edit Element Metadata and Add Children,";
          edit_Element_Metadata_clicked(ref msg);
          break;
        case "Edit Element Metadata and Remove Children":
          msg.content += "Edit Element Metadata and Remove Children,";
          edit_Element_Metadata_clicked(ref msg);
          break;
        case "Edit Element Metadata and Edit Payload":
          msg.content += "Edit Element Metadata and Edit Payload,";
          edit_Element_Metadata_clicked(ref msg);
          break;
        case "Delete Element":
          msg.content += "Delete Element,";
          delete_Element(ref msg);
          break;
        case "Persist Database":
          msg.content += "Persist Database,";
          persist_Database(ref msg);
          break;
        case "Restore Database":
          msg.content += "Restore Database,";
          restore_Database(ref msg);
          break;
      }
    }
    
    //---------< add data according to query to message >-------------
    private void time_Date_Interval(ref Message msg)
    {
      DateTime date = new DateTime();
      if (S_Date_Value.Text != "Timestamp (MM/DD/YYYY HH:MM:SS AM/PM)" && S_Date_Value.Text != null && DateTime.TryParse(S_Date_Value.Text, out date))
      {
        msg.content += "sdate-time," + date.ToString() + ",";
        if (E_Date_Value.Text == "Timestamp (MM/DD/YYYY HH:MM:SS AM/PM)")
        {
          msg.content += "edate-time,";
          if (sndr.sendMessage(msg))
          {
            Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
          }
          else
          {
            Result_Box.Items.Insert(0, "Not able to send Message");
          }
        }
        else if (E_Date_Value.Text != "Timestamp (MM/DD/YYYY HH:MM:SS AM/PM)" && DateTime.TryParse(E_Date_Value.Text, out date))
        {
          msg.content += "edate-time," + date.ToString() + ",";
          if (sndr.sendMessage(msg))
          {
            Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
          }
          else
          {
            Result_Box.Items.Insert(0, "Not able to send Message");
          }
        }
        else
        {
          Result_Box.Items.Insert(0, "Not a Valid Date");
          return;
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid Date");
        return;
      }
    }
    //---------< add data according to query to message >-------------
    private void string_Metadata(ref Message msg)
    {
      msg.content += "string,";
      if (Pattern_Value.Text != null & Pattern_Value.Text != "Pattern to Search")
      {
        msg.content += Pattern_Value.Text;
        if (sndr.sendMessage(msg))
        {
          Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
        }
        else
        {
          Result_Box.Items.Insert(0, "Not able to send Message");
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid Query");
      }
    }
    //---------< add data according to query to message >-------------
    private void pattern_Matching(ref Message msg)
    {
      msg.content += "pattern,";
      if (Pattern_Value.Text != "Pattern to Search")
      {
        msg.content += Pattern_Value.Text;
        if (sndr.sendMessage(msg))
        {
          Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
        }
        else
        {
          Result_Box.Items.Insert(0, "Not able to send Message");
        }
      }
      else if (Pattern_Value.Text == null)
      {
        msg.content += "";
        if (sndr.sendMessage(msg))
        {
          Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
        }
        else
        {
          Result_Box.Items.Insert(0, "Not able to send Message");
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid Query");
      }
    }
    //---------< add data according to query to message >-------------
    private void search_Children(ref Message msg)
    {
      msg.content += "key,";
      if (Key_Value.Text != null & Key_Value.Text != "Key of an Element")
      {
        msg.content += Key_Value.Text;
        if (sndr.sendMessage(msg))
        {
          Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
        }
        else
        {
          Result_Box.Items.Insert(0, "Not able to send Message");
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid Query");
      }
    }
    //---------< add data according to query to message >-------------
    private void search_Key_Value(ref Message msg)
    {
      msg.content += "key,";
      if (Key_Value.Text != null & Key_Value.Text != "Key of an Element")
      {
        msg.content += Key_Value.Text;
        if (sndr.sendMessage(msg))
        {
          Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
        }
        else
        {
          Result_Box.Items.Insert(0, "Not able to send Message");
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid Query");
      }
    }
    //---------< add data according to query to message >-------------
    private void persist_Database(ref Message msg)
    {
      msg.content += "Destination,";
      if (File_Name_Value.Text != null & File_Name_Value.Text != "Write File Name")
      {
        msg.content += File_Name_Value.Text;
        if (sndr.sendMessage(msg))
        {
          Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
        }
        else
        {
          Result_Box.Items.Insert(0, "Not able to send Message");
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid File Name");
      }
    }
    //---------< add data according to query to message >-------------
    private void restore_Database(ref Message msg)
    {
      msg.content += "Source,";
      if (File_Name_Value.Text != null & File_Name_Value.Text != "Write File Name")
      {
        msg.content += File_Name_Value.Text;
        if (sndr.sendMessage(msg))
        {
          Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
        }
        else
        {
          Result_Box.Items.Insert(0, "Not able to send Message");
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid File Name");
      }
    }
    //---------< add data according to query to message >-------------
    private void delete_Element(ref Message msg)
    {
      msg.content += "key,";
      if (Key_Value.Text != null & Key_Value.Text != "Key of an Element")
      {
        if (!validate_Key(Key_Value.Text))
        {
          Result_Box.Items.Insert(0, "Not a valid key");
          return;
        }
        msg.content += Key_Value.Text;
        if (sndr.sendMessage(msg))
        {
          Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
        }
        else
        {
          Result_Box.Items.Insert(0, "Not able to send Message");
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid Query");
      }
    }

    private bool validate_Key(string text)
    {
      if (!flag)
      {
        int temp = 0;
        return int.TryParse(text, out temp);
      }
      return true;
    }

    private bool validate_Children(string text)
    {
      if (!flag)
      {
        int temp = 0;
        return int.TryParse(text, out temp);
      }
      return true;
    }
    //---------< add data according to query to message >-------------
    private void edit_Element_Metadata_clicked(ref Message msg)
    {
      msg.content += "key,";
      if (Key_Value.Text != null & Key_Value.Text != "Key of an Element")
      {
        if (!validate_Key(Key_Value.Text))
        {
          Result_Box.Items.Insert(0, "Not a valid key");
          return;
        }
        msg.content += Key_Value.Text + ",name," + Name_Value.Text + ",descr," + Descr_Value.Text + ",timestamp,";
        DateTime date = new DateTime();
        if (Timestamp_Value.Text != "Timestamp (MM/DD/YYYY HH:MM:SS AM/PM)" & Timestamp_Value.Text != null & DateTime.TryParse(Timestamp_Value.Text, out date))
        {
          msg.content += date.ToString() + ",";
        }
        else
        {
          Result_Box.Items.Insert(0, "Not a Valid Date");
          return;
        }
        if (!addChildren(ref msg))
        {
          return;
        }
        if (!addPayload(ref msg))
        {
          return;
        }

        if (sndr.sendMessage(msg))
        {
          Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
        }
        else
        {
          Result_Box.Items.Insert(0, "Not able to send Message");
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid Query");
      }
    }
    //---------< add data according to query to message >-------------
    private void insert_Element_clicked(ref Message msg)
    {
      msg.content += "key,";
      if (Key_Value.Text != null & Key_Value.Text != "Key of an Element")
      {
        if (!validate_Key(Key_Value.Text))
        {
          Result_Box.Items.Insert(0, "Not a valid key");
          return;
        }
        msg.content += Key_Value.Text + ",name," + Name_Value.Text + ",descr," + Descr_Value.Text + ",timestamp,";
        DateTime date = new DateTime();
        if (Timestamp_Value.Text != "Timestamp (MM/DD/YYYY HH:MM:SS AM/PM)" & Timestamp_Value.Text != null & DateTime.TryParse(Timestamp_Value.Text, out date))
        {
          msg.content += date.ToString() + ",";
        }
        else
        {
          Result_Box.Items.Insert(0, "Not a Valid Date");
          return;
        }
        if (!addChildren(ref msg))
        {
          return;
        }
        if (!addPayload(ref msg))
        {
          return;
        }
        if (sndr.sendMessage(msg))
        {
          Result_Box.Items.Insert(0, "Sending Message: " + msg.content);
        }
        else
        {
          Result_Box.Items.Insert(0, "Not able to send Message");
        }
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid Query");
      }
    }
    //---------< add payload to query message >-------------
    private bool addPayload(ref Message msg)
    {
      msg.content += "payload,";
      if (Number_of_Payload.Text != "Payload (String,String)" && Number_of_Payload.Text != null)
      {
        if (Number_of_Payload.Text.Contains(','))
        {
          String[] payloadList = Number_of_Payload.Text.Split(',');
          msg.content += payloadList.Length + ",";
          foreach (string payload in payloadList)
          {
            msg.content += payload + ",";
          }
        }
        else
        {
          msg.content += "1," + Number_of_Payload.Text.ToString() + ",";
        }
        return true;
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid Payload");
        return false;
      }
    }
    //---------< add children to query message >-------------
    private bool addChildren(ref Message msg)
    {
      msg.content += "children,";
      if (Number_Of_Children.Text != "Children (String,String)" && Number_Of_Children.Text != null)
      {
        if (Number_Of_Children.Text.Contains(','))
        {
          String[] children = Number_Of_Children.Text.Split(',');
          msg.content += children.Length + ",";
          foreach (string child in children)
          {
            if (!validate_Children(child))
            {
              Result_Box.Items.Insert(0, "Not a valid children");
              return false;
            }
            msg.content += child + ",";
          }
        }
        else
        {
          if (!validate_Children(Number_Of_Children.Text.ToString()))
          {
            Result_Box.Items.Insert(0, "Not a valid children");
            return false;
          }
          msg.content += "1," + Number_Of_Children.Text.ToString() + ",";
        }
        return true;
      }
      else
      {
        Result_Box.Items.Insert(0, "Not a Valid Child");
        return false;
      }
    }
  }
}
