using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using WPF.database;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls.Primitives;

namespace WPF
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//load data to datatables which are sources for datagrids
			MoviesGridRefresh();
			ClientsGridRefresh();
			OrdersGridRefresh();
			//Fill form comboboxes
			MoviesComboboxRefresh();
			ClientsComboboxRefresh();

			//Admin Panel 2 Comboboxes
			Admin2ComboboxesRefresh();

			MoviesCatalog.ItemsSource = movies_dt.DefaultView;
			MoviesCatalog.AutoGenerateColumns = true;
			MoviesCatalog.CanUserAddRows = false;
			MoviesCatalog.CanUserDeleteRows = false;

			ClientsCatalog.ItemsSource = clients_dt.DefaultView;
			ClientsCatalog.AutoGenerateColumns = true;
			ClientsCatalog.CanUserAddRows = false;
			ClientsCatalog.CanUserDeleteRows = false;

			OrdersCatalog.ItemsSource = orders_dt.DefaultView;
			OrdersCatalog.AutoGenerateColumns = true;
			OrdersCatalog.CanUserAddRows = false;
			OrdersCatalog.CanUserDeleteRows = false;
		}
		DataTable movies_dt = new DataTable();
		DataTable clients_dt = new DataTable();
		DataTable orders_dt = new DataTable();
		public void MoviesGridRefresh() {
			getquery(movies_dt,
					"Select movies.id, movies.name as title, movies.year, movies.duration, movies.age, movies.price," +
					"movies.plot," +
					"formats.name as format," +
					"CONCAT(directors.last_name,' ',directors.first_name) as director," +
					"CONCAT(actors.last_name,' ',actors.first_name) as lead_actor," +
					"countries.name as country, langs.name as language," +
					"movies.left_count, movies.total_count from movies " +
					"join actors on actors.id = movies.actor_id " +
					"join countries on countries.id = movies.country_id " +
					"join langs on langs.id = movies.lang_id " +
					"join directors on directors.id = movies.director_id " +
					"join formats on formats.id = movies.format_id");
		}
		public void ClientsGridRefresh()
		{
			getquery(clients_dt, "Select * from clients");
		}
		public void OrdersGridRefresh()
		{
			getquery(orders_dt, "Select orders.id,CONCAT(clients.last_name,' ',clients.first_name) as client," +
				"movies.name as title,movies.year, orders.rent_date, orders.due_date, orders.return_date " +
				"from orders " +
				"join movies on movies.id=orders.movie_id " +
				"join clients on clients.id=orders.client_id");
		}
		public void MoviesComboboxRefresh() 
		{
			ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
			ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
			ComboboxRefresh(MovieFormFormatNameID, "select name,id from formats");
			ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
			ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
			ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
		}
		public void ClientsComboboxRefresh()
		{
			ComboboxRefresh(OrderFormClientLNFNID, "select last_name,first_name,id from clients");
		}
		public void Admin2ComboboxesRefresh()
		{
			//Actors
			ComboboxRefresh(UpdateActorFNLNID, "select last_name,first_name,id from actors");
			//Directors
			ComboboxRefresh(UpdateDirectorFNLNID, "select last_name,first_name,id from directors");
			//Countries
			ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");
			//Languages
			ComboboxRefresh(UpdateLangNameID, "select name,id from langs");
			//Formats
			ComboboxRefresh(UpdateFormatNameID, "select name,id from formats");
			//Genres
			ComboboxRefresh(UpdateGenreNameID, "select name,id from genres");
		}
		
		//sql panel button
		private void QueryExecuteButton_Click(object sender, RoutedEventArgs e)
		{
			SQLquery();
		}
		//setquery has error feedback
		public void setquery(string query)
		{
			string strConnection = Properties.Settings.Default.WPF_DBConnectionString;
			SqlConnection con = new SqlConnection(strConnection);
			try
			{
				if (con.State == ConnectionState.Closed)
				{
					con.Open();
				}
				SqlCommand NewCmd = con.CreateCommand();
				NewCmd.Connection = con;
				NewCmd.CommandType = CommandType.Text;
				NewCmd.CommandText = query;
				//number of rows affected
				int a = NewCmd.ExecuteNonQuery();
				con.Close();
				if (a == 0)
				{
					//Not affected
					MessageBox.Show("no rows affected");
				}
				else
				{
					//Affected
					MessageBox.Show("rows affected: " + a);
				}
			}
			catch (Exception ex)
			{
				//Not affected
				MessageBox.Show("no rows affected:\n" + ex.ToString());
			}
		}
		//for sql panel; select getquery or setquery based on select existence
		public void SQLquery()
		{
			if (SQLqueryText.Text != string.Empty)
			{
				if (SQLqueryText.Text.Contains("Select"))
				{
					setquery(SQLqueryText.Text);
				}
				else getquery(SQLgrid, SQLqueryText.Text);
			}
		}
		//refresh datatable to refresh grid items
		public void getquery(DataTable dt, string query)
		{
			string strConnection = Properties.Settings.Default.WPF_DBConnectionString;
			SqlConnection con = new SqlConnection(strConnection);

			SqlCommand sqlCmd = new SqlCommand();
			sqlCmd.Connection = con;
			sqlCmd.CommandType = CommandType.Text;
			sqlCmd.CommandText = query;
			SqlDataAdapter sqlDataAdap = new SqlDataAdapter(sqlCmd);

			sqlDataAdap.Fill(dt);
			con.Close();
		}
		//unused
		public void getquery(DataGrid grid, string query)
		{
			string strConnection = Properties.Settings.Default.WPF_DBConnectionString;
			SqlConnection con = new SqlConnection(strConnection);

			SqlCommand sqlCmd = new SqlCommand();
			sqlCmd.Connection = con;
			sqlCmd.CommandType = CommandType.Text;
			sqlCmd.CommandText = query;
			SqlDataAdapter sqlDataAdap = new SqlDataAdapter(sqlCmd);

			DataTable dtRecord = new DataTable();
			sqlDataAdap.Fill(dtRecord);
			con.Close();
			grid.ItemsSource = dtRecord.DefaultView;
		}
		//query for single row to string
		public string getquery(string query)
		{
			string strConnection = Properties.Settings.Default.WPF_DBConnectionString;
			SqlConnection con = new SqlConnection(strConnection);

			SqlCommand sqlCmd = new SqlCommand();
			sqlCmd.Connection = con;
			sqlCmd.CommandType = CommandType.Text;
			sqlCmd.CommandText = query;
			SqlDataAdapter sqlDataAdap = new SqlDataAdapter(sqlCmd);

			DataTable dtRecord = new DataTable();
			sqlDataAdap.Fill(dtRecord);
			con.Close();
			return dtRecord.Rows[0].ToString();
		}
		//Combobox items clear and fill (refresh)
		public void ComboboxRefresh(ComboBox combo,string query) 
		{
			combo.Items.Clear();

			string strConnection = Properties.Settings.Default.WPF_DBConnectionString;
			SqlConnection con = new SqlConnection(strConnection);

			SqlCommand sqlCmd = new SqlCommand();
			sqlCmd.Connection = con;
			sqlCmd.CommandType = CommandType.Text;
			sqlCmd.CommandText = query;
			SqlDataAdapter sqlDataAdap = new SqlDataAdapter(sqlCmd);

			DataTable dtRecord = new DataTable();
			sqlDataAdap.Fill(dtRecord);
			con.Close();
			combo.ItemsSource = dtRecord.DefaultView;
		}
		public int getmovie_id() {
			//get selected item id from selected row
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
			return int.Parse(rowview.Row["id"].ToString());
		}
		public int getclient_id()
		{
			//get selected item id from selected row
			DataRowView rowview = ClientsCatalog.SelectedItem as DataRowView;
			return int.Parse(rowview.Row["id"].ToString());
		}
		public int getorder_id()
		{
			//get selected item id from selected row
			DataRowView rowview = OrdersCatalog.SelectedItem as DataRowView;
			return int.Parse(rowview.Row["id"].ToString());
		}
		//Trailer panel
		//action on trailer load
		private void TrailerInit(object sender, RoutedEventArgs e)
		{
			//seek slider init
			//SeekSlider.Maximum = Trailer.NaturalDuration.TimeSpan.TotalMilliseconds; //null pointer exception
		}
		private void PlayTrailer(object sender, RoutedEventArgs e)
		{
			Trailer.Play();
		}
		private void PauseTrailer(object sender, MouseButtonEventArgs e)
		{
			Trailer.Pause();
		}
		private void StopTrailer(object sender, RoutedEventArgs e)
		{
			Trailer.Stop();
		}
		private void SeekTrailer(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			int SliderValue = (int)SeekSlider.Value;

			TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
			Trailer.Position = ts;
		}
		//Load existing trailer from database using trailer_path on right click menu
		private void LoadTrailer(object sender, RoutedEventArgs e)
		{
			//get selected item id from selected row
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
			string id = rowview.Row["id"].ToString();

			//Set trailer source from query to uri
			string query_result = getquery("select trailer_path from movies where id=" + id);
			if (query_result != null)
			{
				string file = query_result;
				//Trailer file path correctness check
				if (File.Exists(file))
				{
					Uri uri = new Uri(query_result);
					Trailer.Source = uri;
				}
				else
				{
					MessageBox.Show("Trailer file not found. Use set trailer function to select trailer file");
					//Remove bad file path from database
					DeleteTrailer(null, null);
				}
			}
			else MessageBox.Show("Trailer file path not set. Trailer_path is null.");
		}
		//Set trailer file path and update database
		private void SetTrailer(object sender, MouseButtonEventArgs e)
		{
			OpenFileDialog op = new OpenFileDialog();
			op.Title = "Select trailer";
			if (op.ShowDialog() == true)
			{
				Uri uri = new Uri(op.FileName);
				Trailer.Source = uri;
				//add trailer to movie in database
				setquery("update movies set poster=" + uri + " where id=" + getmovie_id());
			}
		}
		private void DeleteTrailer(object sender, MouseButtonEventArgs e)
		{
			Trailer.Source = null;
			//update database
			setquery("update movies set trailer_path=null where id=" + getmovie_id());
		}
		//Poster panel buttons
		private void RemovePoster(object sender, RoutedEventArgs e)
		{
			Poster.Source = null;
			//remove movie poster from database
			setquery("update movies set poster_path=null where id=" + getmovie_id());
			MoviesGridRefresh();
		}
		private void SetPoster(object sender, MouseButtonEventArgs e)
		{
			OpenFileDialog op = new OpenFileDialog();
			op.Title = "Select poster";
			op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
			  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
			  "Portable Network Graphic (*.png)|*.png";
			if (op.ShowDialog() == true)
			{
				//load poster from file path
				Uri uri = new Uri(op.FileName);
				BitmapImage img = new BitmapImage(uri);
				Poster.Source = img;
				//add poster_path to database
				setquery("update movies set poster_path=" + op.FileName + " where id=" + getmovie_id());
				MoviesGridRefresh();
			}
		}
		//Fills existing movie data into form in admin panel (rent movie option)
		public void MovieEditRentFillForm() 
		{
			MovieFormID.Text = getmovie_id().ToString();
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
			MovieFormTitle.Text = rowview.Row["title"].ToString();
			MovieFormYear.Text = rowview.Row["year"].ToString();
			MovieFormPrice.Text = rowview.Row["price"].ToString();
			MovieFormPlot.Text = rowview.Row["plot"].ToString();
			MovieFormActorLNFNID.Text = rowview.Row["lead_actor"].ToString();
			MovieFormDirectorLNFNID.Text = rowview.Row["director"].ToString();
			MovieFormAge.Text = rowview.Row["age"].ToString();
			MovieFormDuration.Text = rowview.Row["duration"].ToString();
			MovieFormCopiesLeft.Text = rowview.Row["left_count"].ToString();
			MovieFormCopiesTotal.Text = rowview.Row["total_count"].ToString();
			MovieFormLangNameID.Text = rowview.Row["language"].ToString();
			MovieFormFormatNameID.Text = rowview.Row["format"].ToString();
			MovieFormGenreNameID.Text = rowview.Row["genre"].ToString();
		}
		//Fills existing client data into form in admin panel (edit client option)
		public void ClientEditFillForm()
		{
			ClientFormID.Text = getclient_id().ToString();
			DataRowView rowview = ClientsCatalog.SelectedItem as DataRowView;
			ClientFormLastName.Text = rowview.Row["last_name"].ToString();
			ClientFormFirstName.Text = rowview.Row["first_name"].ToString();
			ClientFormEmail.Text = rowview.Row["email"].ToString();
			ClientFormPhone.Text = rowview.Row["phone"].ToString();
		}
		//Fills existing order data into form in admin panel (edit order option)
		public void OrderEditFillForm()
		{
			//set order id
			OrderFormID.Text = getorder_id().ToString();
			
			DataRowView rowview = OrdersCatalog.SelectedItem as DataRowView;
			//set movie
			int movie_id = int.Parse(rowview.Row["movie_id"].ToString());
			OrderFormMovieID.Text = getquery("select title from movies where id=" + movie_id) + " " + movie_id;
			//set client
			int client_id = int.Parse(rowview.Row["client_id"].ToString());
			//first name
			OrderFormClientLNFNID.Text = getquery("select first_name from clients where id=" + client_id);
			//last name
			OrderFormClientLNFNID.Text += " " + getquery("select last_name from clients where id=" + client_id);
			//client name
			OrderFormClientLNFNID.Text += " " + client_id;

			//get dates in format dd/mm/yy (30/12/2022)
			//set rent date
			string rent_date = getquery("select convert(varchar, rent_date, 1) from orders where id=" + OrderFormID.Text);
			int dd = int.Parse(rent_date.Split('/')[0]);
			int mm = int.Parse(rent_date.Split('/')[1]);
			int yy = int.Parse(rent_date.Split('/')[2]);
			OrderFormRentDate.SelectedDate=new DateTime(yy, mm, dd);

			//set due date
			string due_date = getquery("select convert(varchar, due_date, 1) from orders where id=" + OrderFormID.Text);
			dd = int.Parse(due_date.Split('/')[0]);
			mm = int.Parse(due_date.Split('/')[1]);
			yy = int.Parse(due_date.Split('/')[2]);
			OrderFormDueDate.SelectedDate = new DateTime(yy, mm, dd);

			//set return date
			string return_date = getquery("select convert(varchar, return_date, 1) from orders where id=" + OrderFormID.Text);
			dd = int.Parse(return_date.Split('/')[0]);
			mm = int.Parse(return_date.Split('/')[1]);
			yy = int.Parse(return_date.Split('/')[2]);
			OrderFormReturnDate.SelectedDate = new DateTime(yy, mm, dd);
		}
		//Submit Buttons
		private void SubmitOrder(object sender, MouseButtonEventArgs e)
		{
			//get movie_id and client_id
			int movie_id=-1,
				client_id=-1;
			if (OrderFormMovieID.Text != "" &&
				OrderFormClientLNFNID.Text != "")
			{
				//split string and get movie id
				movie_id = int.Parse(OrderFormMovieID.Text);
				//split string and get client id
				client_id = int.Parse(OrderFormClientLNFNID.Text.Split(' ').Last());
			}
			else
			{
				//no movie id or client
				MessageBox.Show("Form incomplete: use rent context menu option in movies catalog to fill movie id and select client");
				//return 1;
			}
			//get dates in format dd/mm/yy (30/12/2022)
			//Default date in form is today
			int dd, mm, yy;
			//set due date if not null
			string rent_date = getquery("select ISNULL(due_date,'') from orders where id=" + OrderFormID.Text);
			if (rent_date != "")
			{
				rent_date = getquery("select convert(varchar, rent_date, 1) from orders where id=" + OrderFormID.Text);
				dd = int.Parse(rent_date.Split('/')[0]);
				mm = int.Parse(rent_date.Split('/')[1]);
				yy = int.Parse(rent_date.Split('/')[2]);
				OrderFormRentDate.SelectedDate = new DateTime(yy, mm, dd);
			}
			//set due date if not null
			string due_date = getquery("select ISNULL(due_date,'') from orders where id=" + OrderFormID.Text);
			if (due_date != "")
			{
				due_date = getquery("select convert(varchar, due_date, 1) from orders where id=" + OrderFormID.Text);
				dd = int.Parse(due_date.Split('/')[0]);
				mm = int.Parse(due_date.Split('/')[1]);
				yy = int.Parse(due_date.Split('/')[2]);
				OrderFormDueDate.SelectedDate = new DateTime(yy, mm, dd);
			}
			//set return date if not null
			string return_date = getquery("select ISNULL(return_date,'') from orders where id=" + OrderFormID.Text);
			if (return_date != "")
			{
				return_date = getquery("select convert(varchar, return_date, 1) from orders where id=" + OrderFormID.Text);
				dd = int.Parse(return_date.Split('/')[0]);
				mm = int.Parse(return_date.Split('/')[1]);
				yy = int.Parse(return_date.Split('/')[2]);
				OrderFormReturnDate.SelectedDate = new DateTime(yy, mm, dd);
			}
				//add mode
				if (mode == false && movie_id>=0 && client_id >= 0)
				{
					//set dates if checked
					if (OrderFormDueDateCheck.IsChecked == false && OrderFormReturnDateCheck.IsChecked == false)
					{
						//due_date = "null";
						//return_date = "null";
					//update movie copies left available if any
					int copies_left = int.Parse(getquery("select left_count from movies where id=" + movie_id));
					if (copies_left > 0) { 
						copies_left--;
						setquery("Insert into Orders (client_id,movie_id,rent_date,due_date,return_date) values(" +
							client_id + "," + movie_id + "," +
							"TODATE(" + rent_date + ",'dd/mm/yy')," +
							"null," +
							"null)");
						setquery("update movies set left_count=" + copies_left);
						//refresh grid
						OrdersGridRefresh();
					}
				}
					if (OrderFormDueDateCheck.IsChecked == false && OrderFormReturnDateCheck.IsChecked == true)
					{
						//due_date="null";
						int copies_left = int.Parse(getquery("select left_count from movies where id=" + movie_id));
						if (copies_left > 0)
						{
							copies_left--;
							setquery("Insert into Orders (client_id,movie_id,rent_date,due_date,return_date) values(" +
								client_id + "," + movie_id + "," +
								"TODATE(" + rent_date + ",'dd/mm/yy')," +
								"null," +
								"TODATE(" + return_date + ",'dd/mm/yy'))");
							setquery("update movies set left_count=" + copies_left);
							//refresh grid
							OrdersGridRefresh();
						}
					}
					if (OrderFormDueDateCheck.IsChecked == true && OrderFormReturnDateCheck.IsChecked == false)
					{
						//return_date = "null";
						int copies_left = int.Parse(getquery("select left_count from movies where id=" + movie_id));
						if (copies_left > 0)
						{
							copies_left--;
							setquery("Insert into Orders (client_id,movie_id,rent_date,due_date,return_date) values(" +
								client_id + "," + movie_id + "," +
								"TODATE(" + rent_date + ",'dd/mm/yy')," +
								"TODATE(" + due_date + ",'dd/mm/yy')," +
								"null)");
							setquery("update movies set left_count=" + copies_left);
							//refresh grid
							OrdersGridRefresh();
						}
					}
					if (OrderFormDueDateCheck.IsChecked == true && OrderFormReturnDateCheck.IsChecked == true)
					{
						int copies_left = int.Parse(getquery("select left_count from movies where id=" + movie_id));
						if (copies_left > 0)
						{
							copies_left--;
							setquery("Insert into Orders (client_id,movie_id,rent_date,due_date,return_date) values(" +
								client_id + "," + movie_id + "," +
								"TODATE(" + rent_date + ",'dd/mm/yy')," +
								"TODATE(" + due_date + ",'dd/mm/yy')," +
								"TODATE(" + return_date + ",'dd/mm/yy'))");
							setquery("update movies set left_count=" + copies_left);
							//refresh grid
							OrdersGridRefresh();
						}
					}
				}
				else if (mode == true && movie_id >= 0 && client_id >= 0)//edit mode
				{
					//get order_id
					if (OrderFormID.Text != "")
					{
						int order_id = int.Parse(OrderFormID.Text);
						//set dates if checked and submit
						if (OrderFormDueDateCheck.IsChecked == false && OrderFormReturnDateCheck.IsChecked == false)
						{
							//due_date = "null";
							//return_date = "null";
							setquery("update Orders set " +
									"client_id=" + client_id + ",movie_id=" + movie_id + "," +
									"rent_date=TODATE(" + rent_date + ",'dd/mm/yy')," +
									"due_date=null," +
									"return_date=null");
							//refresh grid
							OrdersGridRefresh();
							//return 0;
						}
						if (OrderFormDueDateCheck.IsChecked == false && OrderFormReturnDateCheck.IsChecked == true)
						{
							//due_date="null";
							setquery("update Orders set " +
									"client_id=" + client_id + ",movie_id=" + movie_id + "," +
									"rent_date=TODATE(" + rent_date + ",'dd/mm/yy')," +
									"due_date=null," +
									"return_date=TODATE(" + return_date + ",'dd/mm/yy')");
							//refresh grid
							OrdersGridRefresh();
							//return 0;
						}
						if (OrderFormDueDateCheck.IsChecked == true && OrderFormReturnDateCheck.IsChecked == false)
						{
							//return_date = "null";
							setquery("update Orders set " +
									"client_id=" + client_id + ",movie_id=" + movie_id + "," +
									"rent_date=TODATE(" + rent_date + ",'dd/mm/yy')," +
									"due_date=TODATE(" + due_date + ",'dd/mm/yy')," +
									"return_date=null");
							//refresh grid
							OrdersGridRefresh();
							//return 0;
						}
						if (OrderFormDueDateCheck.IsChecked == true && OrderFormReturnDateCheck.IsChecked == true)
						{
							setquery("update Orders set" +
									"client_id=" + client_id + ",movie_id=" + movie_id + "," +
									"rent_date=TODATE(" + rent_date + ",'dd/mm/yy')," +
									"due_date=TODATE(" + due_date + ",'dd/mm/yy')," +
									"return_date=TODATE(" + return_date + ",'dd/mm/yy')");
							//refresh grid
							OrdersGridRefresh();
							//return 0;
						}
					}
					else
					{
						//needed values are not present. For edit mode order_id is needed
						MessageBox.Show("Form incomplete: order id not set");
						//return 1;
					}
				}
				else
				MessageBox.Show("Submit failed. Movie and client have to be set.");
				//return 1;
		}
		private void SubmitClient(object sender, MouseButtonEventArgs e)
		{
			//add mode
			if (mode == false)
			{
				if (ClientFormFirstName.Text != "" && ClientFormLastName.Text != "")
				{
					setquery("Insert into clients (phone,email,first_name,last_name)" +
						"values(" + ClientFormPhone.Text + "," + ClientFormEmail.Text +
						"," + ClientFormFirstName.Text + "," + ClientFormLastName.Text + ")");
					//refresh grid
					ClientsGridRefresh();
					ClientsComboboxRefresh();
				}
				else 
				{
					MessageBox.Show("First name and last name have to be filled");
					//return 1;
				}
			}
			//edit mode
			else 
			{
				if (ClientFormFirstName.Text != "" && ClientFormLastName.Text != "")
				{
					int client_id = int.Parse(ClientFormID.Text);
					setquery("update clients set phone=" + ClientFormPhone.Text + ", email=" + ClientFormEmail.Text +
							",first_name=" + ClientFormFirstName.Text + ",last_name=" + ClientFormLastName.Text + " where id=" + client_id);
					//refresh grid
					ClientsGridRefresh();
					ClientsComboboxRefresh();
				}
				else
				{
					MessageBox.Show("First name last name and id have to be filled. To set id use edit context menu option in clients catalog");
					//return 1;
				}
			}
			//return 1;
		}
		private void SubmitMovie(object sender, MouseButtonEventArgs e)
		{
			//get country id from combobox
			int country_id = int.Parse(MovieFormCountryNameID.Text.Split(' ').Last());
			//get year
			string year;
			if (MovieFormYear.Text != "")
			{
				year = MovieFormYear.Text;
			}
			else
			{
				year = null;
			}
			//get duration
			string duration;
			if (MovieFormDuration.Text != "")
			{
				duration = MovieFormDuration.Text;
			}
			else
			{
				duration = null;
			}
			//get age
			string age;
			if (MovieFormAge.Text != "")
			{
				age = MovieFormAge.Text;
			}
			else
			{
				age = null;
			}
			//get total count of copies
			string copies_total;
			if (MovieFormCopiesTotal.Text != "")
			{
				copies_total = MovieFormCopiesTotal.Text;
			}
			else
			{
				copies_total = null;
			}
			//get count of copies left
			string copies_left;
			if (MovieFormCopiesLeft.Text != "")
			{
				copies_left = MovieFormCopiesLeft.Text;
			}
			else
			{
				copies_left = null;
			}
			//get rental price
			string price;
			if (MovieFormPrice.Text != "")
			{
				price = MovieFormPrice.Text;
			}
			else
			{
				price = null;
			}
			//get plot premise
			string plot;
			if (MovieFormPlot.Text != "")
			{
				plot = MovieFormPlot.Text;
			}
			else
			{
				plot = null;
			}
			//get lang_id from combobox
			string lang_id;
			if (MovieFormLangNameID.Text != "")
			{
				lang_id = MovieFormLangNameID.Text.Split(' ').Last();
			}
			else
			{
				lang_id = null;
			}
			//get actor_id from combobox
			string actor_id;
			if (MovieFormActorLNFNID.Text != "")
			{
				actor_id = MovieFormActorLNFNID.Text.Split(' ').Last();
			}
			else
			{
				actor_id = null;
			}
			//get director_id from combobox
			string director_id;
			if (MovieFormDirectorLNFNID.Text != "")
			{
				director_id = MovieFormDirectorLNFNID.Text.Split(' ').Last();
			}
			else
			{
				director_id = null;
			}
			//get format_id from combobox
			string format_id;
			if (MovieFormFormatNameID.Text != "")
			{
				format_id = MovieFormFormatNameID.Text.Split(' ').Last();
			}
			else
			{
				format_id = null;
			}
			//get genre_id from combobox
			string genre_id;
			if (MovieFormFormatNameID.Text != "")
			{
				genre_id = MovieFormGenreNameID.Text.Split(' ').Last();
			}
			else
			{
				genre_id = null;
			}
			//poster_path and trailer_path have to be set in their respective panels

			//add mode
			if (mode == false)
			{
				if (MovieFormTitle.Text != "")
				{
					setquery("Insert into movies (name,year,country_id,duration,age,total_count,price,left_count,plot,lang_id,actor_id,director_id,format_id,poster_path,trailer_path,genre_id) " +
						"values(" + MovieFormTitle.Text + "," + year +
						"," + country_id + "," + duration + "," + age + "," + copies_total + "," + 
						price + "," + copies_left + "," + plot + "," + lang_id + "," + actor_id + "," + director_id +
						 format_id + ",null,null," + genre_id + ")");
					//refresh grid
					ClientsGridRefresh();
				}
				else
				{
					MessageBox.Show("Movie title not set");
					//return 1;
				}
			}
			//edit mode
			else
			{
				//get movie_id from combobox
				string movie_id=null;
				if (MovieFormFormatNameID.Text != "")
				{
					movie_id = MovieFormID.Text;
				}
				else
				{
					MessageBox.Show("Movie id not set. Use edit context menu option from movies catalog");
					//return 1;
				}
				if (MovieFormTitle.Text != "" && movie_id != null)
				{
					setquery("update movies set movie_id=" + movie_id + ",name=" + MovieFormTitle.Text +
						",year=" + year + ",country_id=" + country_id + ",duration=" + duration + ",age=" + age +
						",total_count=" + copies_total + ",price=" + price + ",left_count=" + copies_left +
						",plot=" + plot + ",lang_id=" + lang_id + ",actor_id=" + actor_id + ",director_id=" + director_id +
						",format_id=" + format_id + ")");
					//refresh grid
					ClientsGridRefresh();
				}
				else
				{
					MessageBox.Show("Movie not set");
					//return 1;
				}
			}
			//return 1;
		}
		//Movies right click menu
		private void MovieItem_rent(object sender, RoutedEventArgs e)
		{
			//add mode
			SelectAddMode(null,null);
			//autofill form
			MovieEditRentFillForm();
			MessageBox.Show("Movie data set. Submit in admin panel.");
		}
		private void MovieItem_edit(object sender, RoutedEventArgs e)
		{
			//edit mode
			SelectEditMode(null, null);
			//autofill form
			MovieEditRentFillForm();
			MessageBox.Show("Movie data set. Submit in admin panel.");
		}
		private void MovieItem_delete(object sender, RoutedEventArgs e)
		{
			setquery("delete from movies where id="+getmovie_id());
			MoviesGridRefresh();
		}
		private void MovieItem_poster(object sender, RoutedEventArgs e)
		{
			//load poster_path from db
			string filepath= getquery("select poster from movies where id=" + getmovie_id());
			//load poster file from poster_path
			Uri uri = new Uri(filepath);
			BitmapImage img = new BitmapImage(uri);
			Poster.Source = img;
		}
		private void MovieItem_trailer(object sender, RoutedEventArgs e)
		{
			TrailerInit(null, null);
		}
		//Clients right click menu
		private void ClientItem_edit(object sender, RoutedEventArgs e)
		{
			//edit mode
			SelectEditMode(null, null);
			//autofill form
			ClientEditFillForm();
			MessageBox.Show("Client data set. Submit in admin panel.");
		}
		//Orders right click menu
		private void OrderItem_return(object sender, RoutedEventArgs e)
		{
			//update return date to current day
			setquery("update orders set return_date=GETDATE() where id=" + getorder_id());
			//get movie id from order and update copies_left
			DataRowView rowview = OrdersCatalog.SelectedItem as DataRowView;
			
			int movie_id = int.Parse(rowview.Row["movie_id"].ToString());
			int copies_left = int.Parse(getquery("select left_count from movies where id=" + movie_id));
			setquery("update movies set left_count=" + copies_left + 1 +" where id=" + movie_id);

			OrdersGridRefresh();
		}
		private void OrderItem_edit(object sender, RoutedEventArgs e)
		{
			//edit mode
			SelectEditMode(null, null);
			OrderEditFillForm();
			MessageBox.Show("Order data set. Edit order in admin panel.");
		}
		private void OrderItem_delete(object sender, RoutedEventArgs e)
		{
			setquery("delete from orders where id=" + getorder_id());
			OrdersGridRefresh();
		}
		//Filter movies
		private void FilterMovieTitle(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMoviePrice(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieAge(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieDuration(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieGenre(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieYear(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieCopiesTotal(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieCopiesLeft(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieCountry(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieLang(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieFormat(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieDirector(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterMovieActor(object sender, TextChangedEventArgs e)
		{
			
		}
		//Filter clients
		private void FilterClientLastName(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterClientFirstName(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterClientEmail(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterClientPhone(object sender, TextChangedEventArgs e)
		{
			
		}
		//Filter orders
		private void FilterOrderMovie(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterOrderLastName(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterOrderFirstName(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterOrderGenre(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterOrderYear(object sender, TextChangedEventArgs e)
		{
			
		}
		private void FilterOrderRentDate(object sender, SelectionChangedEventArgs e)
		{
			
		}
		private void FilterOrderDueDate(object sender, SelectionChangedEventArgs e)
		{
			
		}
		private void FilterOrderReturnDate(object sender, SelectionChangedEventArgs e)
		{
			
		}
		//Mode selectors
		//add or edit mode; false is add, true is edit.
		bool mode = false;
		private void SelectAddMode(object sender, MouseButtonEventArgs e)
		{
			mode = false;
			ModeSelected.Text = "add\nmode";
			ModeSelected.Foreground = Brushes.White;
			//set radial background
			RadialGradientBrush radialGradient = new RadialGradientBrush();
			radialGradient.GradientStops.Add(new GradientStop(Colors.Green, 0.8));
			radialGradient.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));
			radialGradient.Freeze();

			ModeSelected.Background = radialGradient;
			SubmitPanel.Background = radialGradient;
		}
		private void SelectEditMode(object sender, MouseButtonEventArgs e)
		{
			mode = true;
			ModeSelected.Text = "edit\nmode";
			ModeSelected.Foreground = Brushes.Yellow;
			//set radial background
			RadialGradientBrush radialGradient = new RadialGradientBrush();
			radialGradient.GradientStops.Add(new GradientStop(Colors.Red, 0.8));
			radialGradient.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));
			radialGradient.Freeze();

			ModeSelected.Background = radialGradient;
			SubmitPanel.Background = radialGradient;
		}
		private void ListTables(object sender, MouseButtonEventArgs e)
		{
			MessageBox.Show("Available tables:\n" +
				"Movies\n" +
				"Clients\n" +
				"Orders\n" +
				"Actors\n" +
				"Directors\n" +
				"Formats\n" +
				"Countries\n" +
				"Langs (languages)");
		}
		//Admin panel 2
		//New row adding
		private void AddActor(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into actors (first_name,last_name) values("+AddActorFirstName+","+AddActorLastName+")");
			//refresh actors combobox
			ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
		}
		private void AddDirector(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into directors (first_name,last_name) values(" + AddActorFirstName + "," + AddActorLastName + ")");
			//refresh directors combobox
			ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
		}
		private void AddCountry(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into countries (name) values(" + AddCountryName + ")");
			//refresh countries combobox
			ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
		}
		private void AddLang(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into langs (name) values(" + AddLangName + ")");
			//refresh langs combobox
			ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
		}
		private void AddFormat(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into formats (name) values(" + AddFormatName + ")");
			//refresh formats combobox
			ComboboxRefresh(MovieFormFormatNameID, "select name,id from format");
		}
		private void AddGenre(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into genres (name) values(" + AddGenreName + ")");
			//refresh genres combobox
			ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
		}
		//Existing row pdating
		private void UpdateActor(object sender, MouseButtonEventArgs e)
		{
			//combobox contains last_name first_name id
			string id = UpdateActorFNLNID.Text.Split(' ').Last();

			setquery("update actors set first_name=" + UpdateActorFirstName + ",last_name=" + UpdateActorLastName
			+ "where id=" + id +")");
			//refresh actors combobox
			ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
		}
		private void UpdateDirector(object sender, MouseButtonEventArgs e)
		{
			//combobox contains last_name first_name id
			string id = UpdateDirectorFNLNID.Text.Split(' ').Last();

			setquery("update directors set first_name=" + UpdateDirectorFirstName + ",last_name=" + UpdateDirectorLastName
			+ "where id=" + id + ")");
			//refresh directors combobox
			ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
		}
		private void UpdateCountry(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string id = UpdateCountryNameID.Text.Split(' ').Last();

			setquery("update countries set name=" + UpdateCountryName + "where id=" + id + ")");
			//refresh countries combobox
			ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
		}
		private void UpdateLang(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string id = UpdateLangNameID.Text.Split(' ').Last();

			setquery("update langs set name=" + UpdateLangName + "where id=" + id + ")");
			//refresh langs combobox
			ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
		}
		private void UpdateFormat(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string id = UpdateFormatNameID.Text.Split(' ').Last();

			setquery("update formats set name=" + UpdateFormatName + "where id=" + id + ")");
			//refresh formats combobox
			ComboboxRefresh(MovieFormFormatNameID, "select name,id from format");
		}
		private void UpdateGenre(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string id = UpdateGenreNameID.Text.Split(' ').Last();

			setquery("update genres set name=" + UpdateGenreName + "where id=" + id + ")");
			//refresh genres combobox
			ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
		}
		//Existing row Deletion
		private void DeleteActor(object sender, MouseButtonEventArgs e)
		{
			//combobox contains last_name first_name id
			string id = UpdateActorFNLNID.Text.Split(' ').Last();

			setquery("delete from actors where id=" + id + ")");
			//refresh actors combobox
			ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
		}
		private void DeleteDirector(object sender, MouseButtonEventArgs e)
		{
			//combobox contains last_name first_name id
			string id = UpdateDirectorFNLNID.Text.Split(' ').Last();

			setquery("delete from directors where id=" + id + ")");
			//refresh directors combobox
			ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
		}
		private void DeleteCountry(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string id = UpdateCountryNameID.Text.Split(' ').Last();

			setquery("delete from countries where id=" + id + ")");
			//refresh countries combobox
			ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
		}
		private void DeleteLang(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string id = UpdateLangNameID.Text.Split(' ').Last();

			setquery("delete from langs where id=" + id + ")");
			//refresh langs combobox
			ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
		}
		private void DeleteFormat(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string id = UpdateFormatNameID.Text.Split(' ').Last();

			setquery("delete from formats where id=" + id + ")");
			//refresh formats combobox
			ComboboxRefresh(MovieFormFormatNameID, "select name,id from format");
		}
		private void DeleteGenre(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string id = UpdateGenreNameID.Text.Split(' ').Last();

			setquery("delete from genres where id=" + id + ")");
			//refresh genres combobox
			ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
		}

	}
}