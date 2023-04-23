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
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Messaging;

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
					"Select movies.id, movies.name as title, movies.year, movies.duration, movies.age," +
					"CONCAT(genres.name,' ',genres.id) as genre," +
					"movies.price," +
					"CONCAT(formats.name,' ',formats.id) as format," +
					"CONCAT(directors.last_name,' ',directors.first_name,' ',directors.id) as director," +
					"CONCAT(actors.last_name,' ',actors.first_name,' ',actors.id) as 'lead actor'," +
					"CONCAT(countries.name,' ',countries.id) as country," +
					"CONCAT(langs.name,' ',langs.id) as language," +
					"movies.left_count as 'left copies'," +
					"movies.total_count as 'all copies',movies.plot " +
					"from movies " +
					"join actors on actors.id = movies.actor_id " +
					"join countries on countries.id = movies.country_id " +
					"join langs on langs.id = movies.lang_id " +
					"join directors on directors.id = movies.director_id " +
					"join formats on formats.id = movies.format_id " +
					"join genres on genres.id = movies.genre_id");
		}
		public void ClientsGridRefresh()
		{
			getquery(clients_dt, "Select id,last_name as 'last name', first_name as 'first name',phone, email from clients");
		}
		public void OrdersGridRefresh()
		{
			getquery(orders_dt, "Select orders.id,CONCAT(clients.last_name,' ',clients.first_name,' ',clients.id) as client," +
				"CONCAT(movies.name,' ',movies.id) as movie,movies.year, orders.rent_date as 'rent date', orders.due_date as 'due date', orders.return_date as 'return date' " +
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
			ComboboxRefresh(UpdateActorLNFNID, "select last_name,first_name,id from actors");
			//Directors
			ComboboxRefresh(UpdateDirectorLNFNID, "select last_name,first_name,id from directors");
			//Countries
			ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");
			//Languages
			ComboboxRefresh(UpdateLangNameID, "select name,id from langs");
			//Formats
			ComboboxRefresh(UpdateFormatNameID, "select name,id from formats");
			//Genres
			ComboboxRefresh(UpdateGenreNameID, "select name,id from genres");
		}
		//sql panel submit button
		private void QueryExecuteButton_Click(object sender, RoutedEventArgs e)
		{
			SQLquery();
		}
		//nonquery with error feedback
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
			dt.Clear();

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
		//query to grid used in sql panel
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
		//old query for single row to string
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
		public void ComboboxRefresh(ComboBox combo, string query)
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

			//populate rows
			combo.Items.Add("null");
			string row = "";
			for (int i = 0; i < dtRecord.Rows.Count; i++)
			{
				row = "";
				for (int j = 0; j < dtRecord.Columns.Count; j++)
				{
					row += dtRecord.Rows[i][j].ToString()+" ";
				}
				combo.Items.Add(row.Trim());
			}
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
			SelectAddMode(null, null);
			OrderFormMovieID.Text = getmovie_id().ToString();
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
			MovieFormPrice.Text = rowview.Row["price"].ToString();
			//comboboxes
		}
		//Fills existing client data into form in admin panel (edit client option)
		public void ClientEditFillForm()
		{
			SelectEditMode(null, null);
			ClientFormID.Text = getclient_id().ToString();
			DataRowView rowview = ClientsCatalog.SelectedItem as DataRowView;
			ClientFormLastName.Text = rowview.Row["last name"].ToString();
			ClientFormFirstName.Text = rowview.Row["first name"].ToString();
			ClientFormEmail.Text = rowview.Row["email"].ToString();
			ClientFormPhone.Text = rowview.Row["phone"].ToString();
		}
		//Fills existing order data into form in admin panel (edit order option)
		public void OrderEditFillForm()
		{
			SelectEditMode(null, null);
			DataRowView rowview = OrdersCatalog.SelectedItem as DataRowView;
			//get order id
			OrderFormID.Text = getorder_id().ToString();
			
			//get movie id
			string movie_id = rowview.Row["movie"].ToString().Split().Last();
			OrderFormMovieID.Text = movie_id;

			//get client name id
			string client = rowview.Row["client"].ToString();
			OrderFormClientLNFNID.Text = client;
			
			//get rent date
			string rent_date = rowview.Row["rent date"].ToString();
			OrderFormRentDate.Text = rent_date;
			/*
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"select convert(varchar, ISNULL(rent_date,CONVERT(datetime,'01/01/1970',103)), 103) from orders where id=@id";
				var cmd = new SqlCommand(sql, connection);
				cmd.Parameters.AddWithValue("@id", getorder_id());
				rent_date = cmd.ExecuteScalar().ToString();
				int dd = int.Parse(rent_date.Split('/')[0]);
				int mm = int.Parse(rent_date.Split('/')[1]);
				int yy = int.Parse(rent_date.Split('/')[2]);
				//set date if not null
				if (dd != 1 && mm != 1 && yy != 1970)
					OrderFormRentDate.SelectedDate = new DateTime(yy, mm, dd);
			}
			*/
			//get due date
			string due_date = rowview.Row["due date"].ToString();
			OrderFormDueDate.Text = due_date;
			/*
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"select convert(varchar, ISNULL(due_date,CONVERT(datetime,'01/01/1970',103)), 103) from orders where id=@id";
				var cmd = new SqlCommand(sql, connection);
				cmd.Parameters.AddWithValue("@id", getorder_id());
				due_date = cmd.ExecuteScalar().ToString();
				int dd = int.Parse(due_date.Split('/')[0]);
				int mm = int.Parse(due_date.Split('/')[1]);
				int yy = int.Parse(due_date.Split('/')[2]);
				//set date if not null
				if (dd != 1 && mm != 1 && yy != 1970)
					OrderFormDueDate.SelectedDate = new DateTime(yy, mm, dd);
			}
			*/
			//get return date
			string return_date = rowview.Row["return date"].ToString();
			OrderFormReturnDate.Text = return_date;
			/*
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"select convert(varchar, ISNULL(return_date,CONVERT(datetime,'01/01/1970',103)), 103) from orders where id=@id";
				var cmd = new SqlCommand(sql, connection);
				cmd.Parameters.AddWithValue("@id", getorder_id());
				return_date = cmd.ExecuteScalar().ToString();
				int dd = int.Parse(return_date.Split('/')[0]);
				int mm = int.Parse(return_date.Split('/')[1]);
				int yy = int.Parse(return_date.Split('/')[2]);
				//set date if not null
				if (dd != 1 && mm != 1 && yy != 1970)
					OrderFormReturnDate.SelectedDate = new DateTime(yy, mm, dd);
			}
			*/
		}
		//Submit Buttons
		private void SubmitOrder(object sender, MouseButtonEventArgs e)
		{
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			//Console.WriteLine(OrderFormClientLNFNID.Text);
			foreach (string item in OrderFormClientLNFNID.Items)
			{
				//Console.WriteLine(item+" "+ OrderFormClientLNFNID.Text);
				exists = item == OrderFormClientLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) OrderFormClientLNFNID.Text = "null";

			//get movie_id and client_id
			string movie_id = null,
				client_id = null;
			if (OrderFormMovieID.Text != "" &&
				OrderFormClientLNFNID.Text != "null")
			{
				//split string and get movie id
				movie_id = OrderFormMovieID.Text;
				//split string and get client id
				client_id = OrderFormClientLNFNID.Text.Split(' ').Last();
			}
			else
			{
				//no movie id or client
				MessageBox.Show("Form incomplete: use rent context menu option in movies catalog to fill movie id and select client using combobox");
			}
			//client, movie and rent date must be present
			if (OrderFormClientLNFNID.Text != "" && OrderFormClientLNFNID.Text != null &&
				OrderFormMovieID.Text != "" && OrderFormMovieID.Text != null &&
				OrderFormRentDate.Text != "" && OrderFormRentDate.Text != null)
			{
				//get number of copies left available
				int copies_left = 0;
				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"select left_count from movies where id=@id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", movie_id);
					copies_left = int.Parse(cmd.ExecuteScalar().ToString());
				}
				//add (rent) mode if copies available
				if (mode == false)
				{
					if (copies_left > 0)
					{
							//new movie order
							using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
							{
								connection.Open();
								var sql = @"Insert into Orders (client_id,movie_id,rent_date,due_date,return_date) " +
									"values(@client_id,@movie_id," +
									"@rent_date,@due_date,@return_date)";
								using (var cmd = new SqlCommand(sql, connection))
								{
									cmd.Parameters.AddWithValue("@client_id", client_id);
									cmd.Parameters.AddWithValue("@movie_id", movie_id);
									cmd.Parameters.AddWithValue("@rent_date", OrderFormRentDate.Text);
									cmd.Parameters.AddWithValue("@due_date", OrderFormDueDate.Text);
									cmd.Parameters.AddWithValue("@return_date", OrderFormReturnDate.Text);
									MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
									OrdersGridRefresh();
									//decrement movie copies available when client rents a copy
									copies_left--;
								}
								//update copies left
								sql = @"update movies set left_count=@left_count where id=@id";
								using (var cmd = new SqlCommand(sql, connection))
								{
									cmd.Parameters.AddWithValue("@id", movie_id);
									cmd.Parameters.AddWithValue("@left_count", copies_left);
									cmd.ExecuteNonQuery();
									MoviesGridRefresh();
								}
							}
					}
					else MessageBox.Show("No copies left to rent");
				}//edit mode
				else if (mode == true)
				{
					//get order_id
					if (OrderFormID.Text != "")
					{
						string order_id = OrderFormID.Text;
							using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
							{
								connection.Open();
								var sql = @"update orders set client_id=@client_id, movie_id=@movie_id," +
									"rent_date=@rent_date, due_date=@due_date, return_date=@return_date " +
									"where id=@id";
								using (var cmd = new SqlCommand(sql, connection))
								{
									cmd.Parameters.AddWithValue("@id", order_id);
									cmd.Parameters.AddWithValue("@client_id", client_id);
									cmd.Parameters.AddWithValue("@movie_id", movie_id);
									cmd.Parameters.AddWithValue("@rent_date", OrderFormRentDate.Text);
									cmd.Parameters.AddWithValue("@due_date", OrderFormDueDate.Text);
									cmd.Parameters.AddWithValue("@return_date", OrderFormReturnDate.Text);
									MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
								}
							}
							OrdersGridRefresh();
					}
					else
					{
						//needed values are not present. For edit mode order_id is needed
						MessageBox.Show("Form incomplete: order id not set");
					}
				}
			}
			else MessageBox.Show("No client, movie or rent date selected");
		}
		private void SubmitClient(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//phone
			Regex rg = new Regex(@"^[\+]*?[0-9\-]+$");
			MatchCollection matched = rg.Matches(ClientFormPhone.Text);
			if (matched.Count == 0) 
				ClientFormPhone.Text = "";
			else
				ClientFormPhone.Text.Trim();

			//last name
			rg = new Regex(@"^[a-zA-Z-\s]+$");
			matched = rg.Matches(ClientFormLastName.Text);
			if (matched.Count == 0) 
				ClientFormLastName.Text = "";
			else 
				ClientFormLastName.Text.Trim();

			//first name
			rg = new Regex(@"^[a-zA-Z-\s]+$");
			matched = rg.Matches(ClientFormFirstName.Text);
			if (matched.Count == 0) 
				ClientFormFirstName.Text = "";
			else 
				ClientFormFirstName.Text.Trim();

			//email
			rg = new Regex(@"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+");
			matched = rg.Matches(ClientFormEmail.Text);
			if (matched.Count == 0)
				ClientFormEmail.Text = "";
			else
				ClientFormEmail.Text.Trim();

			//ensure needed data is set
			if (ClientFormFirstName.Text != "" && ClientFormLastName.Text != "" &&
					ClientFormFirstName.Text != null && ClientFormLastName.Text != null) 
			{
				//add mode
				if (mode == false)
				{
						using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
						{
							connection.Open();
							var sql = @"INSERT INTO clients(phone,email,first_name, last_name) " +
								"VALUES(@phone,@email,@first_name, @last_name)";
							using (var cmd = new SqlCommand(sql, connection))
							{
								cmd.Parameters.AddWithValue("@phone", ClientFormPhone.Text);
								cmd.Parameters.AddWithValue("@email", ClientFormEmail.Text);
								cmd.Parameters.AddWithValue("@first_name", ClientFormFirstName.Text);
								cmd.Parameters.AddWithValue("@last_name", ClientFormLastName.Text);
								MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
								//refresh grid
								ClientsGridRefresh();
								ClientsComboboxRefresh();
							}
						}
				}
				//edit mode
				else if (ClientFormID.Text != "" && ClientFormID.Text != null)
				{
					using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
					{
						connection.Open();
						var sql = @"update clients set phone=@phone, email=@email, first_name=@first_name, last_name=@last_name " +
							"where id=@id";
						using (var cmd = new SqlCommand(sql, connection))
						{
							cmd.Parameters.AddWithValue("@id", ClientFormID.Text);
							cmd.Parameters.AddWithValue("@phone", ClientFormPhone.Text);
							cmd.Parameters.AddWithValue("@email", ClientFormEmail.Text);
							cmd.Parameters.AddWithValue("@first_name", ClientFormFirstName.Text);
							cmd.Parameters.AddWithValue("@last_name", ClientFormLastName.Text);
							MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
							//refresh grid
							ClientsGridRefresh();
							ClientsComboboxRefresh();
						}
					}
				}
				else MessageBox.Show("To set id use edit context menu option in clients catalog");
			}
			else MessageBox.Show("First name and last name must be filled correctly");
		}
		private void SubmitMovie(object sender, MouseButtonEventArgs e)
		{
			//remove bad values from textboxes
			if (MovieFormAge.Text.Any(char.IsLetter)) MovieFormAge.Text = "0";
			if (MovieFormYear.Text.Any(char.IsLetter)) MovieFormYear.Text = "0";
			if (MovieFormDuration.Text.Any(char.IsLetter)) MovieFormDuration.Text = "0";
			if (MovieFormPrice.Text.Any(char.IsLetter)) MovieFormPrice.Text = "0";
			if (MovieFormCopiesLeft.Text.Any(char.IsLetter)) MovieFormCopiesLeft.Text = "1";
			if (MovieFormCopiesTotal.Text.Any(char.IsLetter)) MovieFormCopiesTotal.Text = "1";

			//null on nonexistent data in comboboxes
			//country
			bool exists = false;
			foreach (string item in MovieFormCountryNameID.Items)
			{
				exists = item == MovieFormCountryNameID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormCountryNameID.Text = "null";

			//lang
			exists = false;
			foreach (string item in MovieFormLangNameID.Items)
			{
				exists = item == MovieFormLangNameID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormLangNameID.Text = "null";

			//genre
			exists = false;
			foreach (string item in MovieFormGenreNameID.Items)
			{
				exists = item == MovieFormGenreNameID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormGenreNameID.Text = "null";

			//director
			exists = false;
			foreach (string item in MovieFormDirectorLNFNID.Items)
			{
				exists = item == MovieFormDirectorLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormDirectorLNFNID.Text = "null";

			//actor
			exists = false;
			foreach (string item in MovieFormActorLNFNID.Items)
			{
				exists = item == MovieFormActorLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormActorLNFNID.Text = "null";

			//format
			exists = false;
			foreach (string item in MovieFormFormatNameID.Items)
			{
				exists = item == MovieFormFormatNameID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormFormatNameID.Text = "null";

			//get country id from combobox
			string country_id = "1";
			if (MovieFormCountryNameID.Text != "null")	country_id = MovieFormCountryNameID.Text.Split(' ').Last();
			//get year
			string year = "0";
			if (MovieFormYear.Text != "")				year = MovieFormYear.Text;
			//get duration
			string duration = "0";
			if (MovieFormDuration.Text != "")			duration = MovieFormDuration.Text;
			//get age
			string age = "0";
			if (MovieFormAge.Text != "")				age = MovieFormAge.Text;
			//get total count of copies
			string copies_total = "1";
			if (MovieFormCopiesTotal.Text != "")		copies_total = MovieFormCopiesTotal.Text;
			//get count of copies left
			string copies_left = "1";
			if (MovieFormCopiesLeft.Text != "")			copies_left = MovieFormCopiesLeft.Text;
			//get rental price
			string price = "0";
			if (MovieFormPrice.Text != "")				price = MovieFormPrice.Text;
			//get plot premise
			string plot = "Premise unknown";
			if (MovieFormPlot.Text != "")				plot = MovieFormPlot.Text;
			//get lang_id from combobox
			string lang_id = "1";
			if (MovieFormLangNameID.Text != "null")		lang_id = MovieFormLangNameID.Text.Split(' ').Last();
			//get actor_id from combobox
			string actor_id = "1";
			if (MovieFormActorLNFNID.Text != "null")	actor_id = MovieFormActorLNFNID.Text.Split(' ').Last();
			//get director_id from combobox
			string director_id = "1";
			if (MovieFormDirectorLNFNID.Text != "null") director_id = MovieFormDirectorLNFNID.Text.Split(' ').Last();
			//get format_id from combobox
			string format_id = "1";
			if (MovieFormFormatNameID.Text != "null")	format_id = MovieFormFormatNameID.Text.Split(' ').Last();
			//get genre_id from combobox
			string genre_id = "1";
			if (MovieFormFormatNameID.Text != "null")	genre_id = MovieFormGenreNameID.Text.Split(' ').Last();
			//poster_path and trailer_path must be set in their respective tabs
			
			//add mode
			if (mode == false)
			{
				if (MovieFormTitle.Text != null)
				{
					using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
					{
						connection.Open();
						var sql = @"INSERT INTO movies (name,year,country_id,duration,age,total_count,price,left_count,plot,lang_id,actor_id,director_id,format_id,genre_id) " +
							"VALUES(@name,@year,@country_id,@duration,@age,@total_count,@price,@left_count,@plot,@lang_id,@actor_id,@director_id,@format_id,@genre_id)";
						using (var cmd = new SqlCommand(sql, connection))
						{
							cmd.Parameters.AddWithValue("@name", MovieFormTitle.Text);
							cmd.Parameters.AddWithValue("@year", year);
							cmd.Parameters.AddWithValue("@country_id", country_id);
							cmd.Parameters.AddWithValue("@duration", duration);
							cmd.Parameters.AddWithValue("@age", age);
							cmd.Parameters.AddWithValue("@total_count", copies_total);
							cmd.Parameters.AddWithValue("@price", price);
							cmd.Parameters.AddWithValue("@left_count", copies_left);
							cmd.Parameters.AddWithValue("@plot", plot);
							cmd.Parameters.AddWithValue("@lang_id", lang_id);
							cmd.Parameters.AddWithValue("@actor_id", actor_id);
							cmd.Parameters.AddWithValue("@director_id", director_id);
							cmd.Parameters.AddWithValue("@format_id", format_id);
							cmd.Parameters.AddWithValue("@genre_id", genre_id);
							MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
						}
					}
					//setquery("Insert into movies (name,year,country_id,duration,age,total_count,price,left_count,plot,lang_id,actor_id,director_id,format_id,genre_id) " +
					//	"values(" + MovieFormTitle.Text + "," + year + "," + country_id + "," + duration + "," + age + "," + copies_total + "," + 
					//	price + "," + copies_left + "," + plot + "," + lang_id + "," + actor_id + "," + director_id + "," +
					//	 format_id + "," + genre_id + ")");
					//refresh grid
					MoviesGridRefresh();
				}
				else
				{
					MessageBox.Show("Movie title not set");
				}
			}
			//edit mode
			else
			{
				//get movie_id from combobox
				string movie_id = null;
				if (MovieFormFormatNameID.Text != "null")
				{
					movie_id = MovieFormID.Text;
				}
				else
				{
					MessageBox.Show("Movie id not set. Use edit context menu option from movies catalog");
				}
				if (MovieFormTitle.Text != null && movie_id != null)
				{
					using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
					{
						connection.Open();
						var sql = @"update movies set " +
							"name=@name," +
							"year=@year," +
							"country_id=@country_id," +
							"duration=@duration," +
							"age=@age," +
							"total_count=@total_count," +
							"price=@price," +
							"left_count=@left_count," +
							"plot=@plot," +
							"lang_id=@lang_id," +
							"actor_id=@actor_id," +
							"director_id=@director_id," +
							"format_id=@format_id," +
							"genre_id=@genre_id " +
							"where id=@id";
						using (var cmd = new SqlCommand(sql, connection))
						{
							cmd.Parameters.AddWithValue("@id", movie_id);
							cmd.Parameters.AddWithValue("@name", MovieFormTitle.Text);
							cmd.Parameters.AddWithValue("@year", year);
							cmd.Parameters.AddWithValue("@country_id", country_id);
							cmd.Parameters.AddWithValue("@duration", duration);
							cmd.Parameters.AddWithValue("@age", age);
							cmd.Parameters.AddWithValue("@total_count", copies_total);
							cmd.Parameters.AddWithValue("@price", price);
							cmd.Parameters.AddWithValue("@left_count", copies_left);
							cmd.Parameters.AddWithValue("@plot", plot);
							cmd.Parameters.AddWithValue("@lang_id", lang_id);
							cmd.Parameters.AddWithValue("@actor_id", actor_id);
							cmd.Parameters.AddWithValue("@director_id", director_id);
							cmd.Parameters.AddWithValue("@format_id", format_id);
							cmd.Parameters.AddWithValue("@genre_id", genre_id);
							MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
						}
					}
					MoviesGridRefresh();
				}
				else
				{
					MessageBox.Show("Movie not set");
				}
			}
		}
		//Movies right click menu
		private void MovieItem_plot(object sender, RoutedEventArgs e)
		{
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
			string plot = rowview.Row["plot"].ToString();
			MessageBox.Show(plot);
		}
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
				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from movies where id=@id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", getmovie_id());
				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
				}
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
		private void ClientItem_delete(object sender, RoutedEventArgs e)
		{
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"delete from clients
						WHERE id = @id";
				var cmd = new SqlCommand(sql, connection);
				cmd.Parameters.AddWithValue("@id", getclient_id());
				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			ClientsGridRefresh();
		}
		//Orders right click menu
		private void OrderItem_return(object sender, RoutedEventArgs e)
		{
			//get selected row
			DataRowView rowview = OrdersCatalog.SelectedItem as DataRowView;
			//check null on return date
			string is_returned = rowview.Row["return date"].ToString();

			//ensure movie is not returned
			if (is_returned != null || is_returned != "")
			{
				//get order id
				int order_id = getorder_id();

				//get movie id
				string movie_id;
				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"select movie_id from orders where id=@id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", order_id);
					movie_id = cmd.ExecuteScalar().ToString();
				}
				//string movie_id = getquery("select movie_id from orders where id=" + order_id);

				//get current copies left
				int copies_left;
				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"select left_count from movies where id=@id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", movie_id);
					copies_left = int.Parse(cmd.ExecuteScalar().ToString());
				}
				//int copies_left = int.Parse(getquery("select left_count from movies where id=" + movie_id)) + 1;

				//add and update movie copies left
				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"update movies set left_count=@left_count WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", movie_id);
					cmd.Parameters.AddWithValue("@left_count", copies_left + 1);
					cmd.ExecuteNonQuery();
				}
				//setquery("update movies set left_count=" + copies_left + " where id=" + movie_id);
				//set movie order return date
				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"update orders set return_date=GETDATE() WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", order_id);
					//cmd.Parameters.AddWithValue("@return_date", "GETDATE()");
					cmd.ExecuteNonQuery();
				}
				//setquery("update orders set return_date=GETDATE() where id=" + getorder_id());
				MoviesGridRefresh();
				OrdersGridRefresh();
			}
			else 
			{
				MessageBox.Show("Movie already returned");
			}
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
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"delete from orders
						WHERE id = @id";
				var cmd = new SqlCommand(sql, connection);
				cmd.Parameters.AddWithValue("@id", getorder_id());
				cmd.ExecuteNonQuery();
			}
			//setquery("delete from orders where id=" + getorder_id());
			OrdersGridRefresh();
		}
		//Filter movies
		private void FilterMovieTitle(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "title like '%" + FilterMovieTitleText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMoviePrice(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "price like '%" + FilterMoviePriceText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieAge(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "age like '%" + FilterMovieAgeText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieDuration(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "duration like '%" + FilterMovieDurationText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieGenre(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "genre like '%" + FilterMovieGenreText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieYear(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "year like '%" + FilterMovieYearText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieCopiesTotal(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "'copies total' like '%" + FilterMovieCopiesTotalText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieCopiesLeft(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "'copies left' like '%" + FilterMovieCopiesLeftText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieCountry(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "country like '%" + FilterMovieCountryText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieLang(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "language like '%" + FilterMovieLangText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieFormat(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "format like '%" + FilterMovieFormatText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieDirector(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "director like '%" + FilterMovieDirectorText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieActor(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "'lead actor' like '%" + FilterMovieActorText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		//Filter clients
		private void FilterClientLastName(object sender, TextChangedEventArgs e)
		{
			try
			{
				clients_dt.DefaultView.RowFilter = "'last name' like '%" + FilterClientLastNameText.Text + "%'";
				//ClientsGridRefresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			/*
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"Select id, last_name as 'last name', first_name as 'first name', phone, email from clients
							where last_name like '%@last_name%'";// and first_name like '%@first_name%' and phone like '%@phone%' and email like '%@email%'";
				var cmd = new SqlCommand(sql, connection);
				cmd.Parameters.AddWithValue("@first_name", FilterClientFirstNameText.Text);
				cmd.Parameters.AddWithValue("@last_name", FilterClientLastNameText.Text);
				cmd.Parameters.AddWithValue("@phone", FilterClientPhoneText.Text);
				cmd.Parameters.AddWithValue("@email", FilterClientEmailText.Text);
				try
				{
					using(SqlDataReader rdr = cmd.ExecuteReader())
					{
						//clients_dt.Clear();
						clients_dt.Load(rdr);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			*/
			//DataView dataView = clients_dt.DefaultView;
			//if (!string.IsNullOrEmpty(FilterClientLastNameText.Text))
			//{
			//	dataView.RowFilter = "'last name' = " + FilterClientLastNameText.Text;
			//}
			//ClientsCatalog.ItemsSource = dataView;
			/*
			try
			{
				getquery(orders_dt, "Select orders.id,CONCAT(clients.last_name,' ',clients.first_name,' ',clients.id) as client," +
				"CONCAT(movies.name,' ',movies.id) as movie,movies.year, orders.rent_date as 'rent date', orders.due_date as 'due date', orders.return_date as 'return date' " +
				"from orders " +
				"join movies on movies.id=orders.movie_id " +
				"join clients on clients.id=orders.client_id " +
				"where clients.last_name like '@FilterClientLastNameText.Text%'");

				DataTable dt = new DataTable();
				string CommandQuery = "SELECT *,row_number() over (order by ProductId) as DisplayOrder FROM Products";
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["testConnectionString"].ConnectionString);
				SqlCommand cmd = new SqlCommand(CommandQuery, conn);
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				cmd.Connection.Open();
				da.Fill(dt);
				DataView dv1 = dt.DefaultView;
				dv1.RowFilter = " ProductId = 1 or ProductId = 2 or ProductID = 3";
				DataTable dtNew = dv1.ToTable();

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			*/
		}
		private void FilterClientFirstName(object sender, TextChangedEventArgs e)
		{
			try
			{
				clients_dt.DefaultView.RowFilter = "'first name' like '%" + FilterClientFirstNameText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			
		}
		private void FilterClientEmail(object sender, TextChangedEventArgs e)
		{
			try
			{
				clients_dt.DefaultView.RowFilter = "'email' like '%" + FilterClientEmailText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			
		}
		private void FilterClientPhone(object sender, TextChangedEventArgs e)
		{
			try
			{
				clients_dt.DefaultView.RowFilter = "'phone' like '%" + FilterClientPhoneText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		//Filter orders
		private void FilterOrderMovie(object sender, TextChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderMovieText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderLastName(object sender, TextChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderLastNameText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderFirstName(object sender, TextChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderFirstNameText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderGenre(object sender, TextChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderGenreText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderYear(object sender, TextChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderYearText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderRentStartDate(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderRentStartDateText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderRentStopDate(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderRentStopDateText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderDueStartDate(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderDueStartDateText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderDueStopDate(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderDueStopDateText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderReturnStartDate(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderReturnStartDateText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderReturnStopDate(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'phone' like '%" + FilterOrderReturnStopDateText.Text + "%'";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
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
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"INSERT INTO actors(first_name, last_name) VALUES(@first_name, @last_name)";
				using (var cmd = new SqlCommand(sql, connection))
				{
					cmd.Parameters.AddWithValue("@first_name", AddActorFirstName.Text);
					cmd.Parameters.AddWithValue("@last_name", AddActorLastName.Text);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into actors (first_name,last_name) values(" + AddActorFirstName.Text + "," + AddActorLastName.Text +")");
			//refresh actors combobox
			ComboboxRefresh(UpdateActorLNFNID, "select last_name,first_name,id from actors");
			ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
		}
		private void AddDirector(object sender, MouseButtonEventArgs e)
		{
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"INSERT INTO directors(first_name, last_name) VALUES(@first_name, @last_name)";
				using (var cmd = new SqlCommand(sql, connection))
				{
					cmd.Parameters.AddWithValue("@first_name", AddDirectorFirstName.Text);
					cmd.Parameters.AddWithValue("@last_name", AddDirectorLastName.Text);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into directors (first_name,last_name) values(" + AddActorFirstName.Text + "," + AddActorLastName.Text + ")");
			//refresh directors combobox
			ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
			ComboboxRefresh(UpdateDirectorLNFNID, "select last_name,first_name,id from directors");
		}
		private void AddCountry(object sender, MouseButtonEventArgs e)
		{
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"INSERT INTO countries(name) VALUES(@name)";
				using (var cmd = new SqlCommand(sql, connection))
				{
					cmd.Parameters.AddWithValue("@name", AddCountryName.Text);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into countries (name) values(" + AddCountryName.Text + ")");
			//refresh countries combobox
			ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
			ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");
		}
		private void AddLang(object sender, MouseButtonEventArgs e)
		{
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"INSERT INTO langs(name) VALUES(@name)";
				using (var cmd = new SqlCommand(sql, connection))
				{
					cmd.Parameters.AddWithValue("@name", AddLangName.Text);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			setquery("Insert into langs (name) values(" + AddLangName.Text + ")");
			//refresh langs combobox
			ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
			ComboboxRefresh(UpdateLangNameID, "select name,id from langs");
		}
		private void AddFormat(object sender, MouseButtonEventArgs e)
		{
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"INSERT INTO formats(name) VALUES(@name)";
				using (var cmd = new SqlCommand(sql, connection))
				{
					cmd.Parameters.AddWithValue("@name", AddFormatName.Text);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into formats (name) values(" + AddFormatName.Text + ")");
			//refresh formats combobox
			ComboboxRefresh(MovieFormFormatNameID, "select name,id from format");
			ComboboxRefresh(UpdateFormatNameID, "select name,id from format");
		}
		private void AddGenre(object sender, MouseButtonEventArgs e)
		{
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"INSERT INTO genres(name) VALUES(@name)";
				using (var cmd = new SqlCommand(sql, connection))
				{
					cmd.Parameters.AddWithValue("@name", AddGenreName.Text);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into genres (name) values(" + AddGenreName.Text + ")");
			//refresh genres combobox
			ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
			ComboboxRefresh(UpdateGenreNameID, "select name,id from genres");
		}
		//Existing row pdating
		private void UpdateActor(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateActorLNFNID.Items)
			{
				exists = item == UpdateActorLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateActorLNFNID.Text = "null";

			if (UpdateActorLNFNID.Text != "null")
			{
				//combobox contains last_name first_name id
				string actor_id = UpdateActorLNFNID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"UPDATE actors
						SET last_name = @last_name, first_name = @first_name
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@last_name", UpdateActorLastName.Text);
					cmd.Parameters.AddWithValue("@first_name", UpdateActorFirstName.Text);
					cmd.Parameters.AddWithValue("@id", actor_id);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//setquery("update actors set first_name=" + UpdateActorFirstName.Text + ",last_name=" + UpdateActorLastName.Text
				//+ "where id=" + actor_id + ")");
				//refresh actors combobox
				ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
				ComboboxRefresh(UpdateActorLNFNID, "select last_name,first_name,id from actors");
			}
			else MessageBox.Show("No item selected");
		}
		private void UpdateDirector(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateDirectorLNFNID.Items)
			{
				exists = item == UpdateDirectorLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateDirectorLNFNID.Text = "null";

			if (UpdateDirectorLNFNID.Text != "null")
			{
				//combobox contains last_name first_name id
				string director_id = UpdateDirectorLNFNID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"UPDATE directors
						SET last_name = @last_name, first_name = @first_name
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@last_name", UpdateDirectorLastName.Text);
					cmd.Parameters.AddWithValue("@first_name", UpdateDirectorFirstName.Text);
					cmd.Parameters.AddWithValue("@id", director_id);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//setquery("update directors set first_name=" + UpdateDirectorFirstName.Text + ",last_name=" + UpdateDirectorLastName.Text
				//+ "where id=" + director_id + ")");
				//refresh directors combobox
				ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
				ComboboxRefresh(UpdateDirectorLNFNID, "select last_name,first_name,id from directors");
			}
			else MessageBox.Show("No item selected");
		}
		private void UpdateCountry(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateCountryNameID.Items)
			{
				exists = item == UpdateCountryNameID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateCountryNameID.Text = "null";

			if (UpdateCountryNameID.Text != "null")
			{
				//combobox contains name id
				string country_id = UpdateCountryNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"UPDATE countries
						SET name = @name,
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@name", UpdateCountryName.Text);
					cmd.Parameters.AddWithValue("@id", country_id);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//setquery("update countries set name=" + UpdateCountryName.Text + "where id=" + country_id + ")");
				//refresh countries combobox
				ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
				ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");
			}
			else MessageBox.Show("No item selected");
		}
		private void UpdateLang(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateLangNameID.Items)
			{
				exists = item == UpdateLangNameID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateLangNameID.Text = "null";

			if (UpdateLangNameID.Text != "null")
			{
				//combobox contains name id
				string lang_id = UpdateLangNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"UPDATE langs
						SET name = @name,
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@name", UpdateLangName.Text);
					cmd.Parameters.AddWithValue("@id", lang_id);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//setquery("update langs set name=" + UpdateLangName.Text + "where id=" + lang_id + ")");
				//refresh langs combobox
				ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
				ComboboxRefresh(UpdateLangNameID, "select name,id from langs");
			}
			else MessageBox.Show("No item selected");
		}
		private void UpdateFormat(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateFormatNameID.Items)
			{
				exists = item == UpdateFormatNameID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateFormatNameID.Text = "null";

			if (UpdateFormatNameID.Text != "null")
			{
				//combobox contains name id
				string format_id = UpdateFormatNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"UPDATE formats
						SET name = @name,
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@name", UpdateFormatName.Text);
					cmd.Parameters.AddWithValue("@id", format_id);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//setquery("update formats set name=" + UpdateFormatName.Text + "where id=" + format_id + ")");
				//refresh formats combobox
				ComboboxRefresh(MovieFormFormatNameID, "select name,id from format");
				ComboboxRefresh(UpdateFormatNameID, "select name,id from format");
			}
			else MessageBox.Show("No item selected");
		}
		private void UpdateGenre(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateGenreNameID.Items)
			{
				exists = item == UpdateGenreNameID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateGenreNameID.Text = "null";

			if (UpdateGenreNameID.Text != "null")
			{
				//combobox contains name id
				string genre_id = UpdateGenreNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"UPDATE genres
						SET name = @name,
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@name", UpdateGenreName.Text);
					cmd.Parameters.AddWithValue("@id", genre_id);
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//setquery("update genres set name=" + UpdateGenreName.Text + "where id=" + genre_id + ")");
				//refresh genres combobox
				ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
				ComboboxRefresh(UpdateGenreNameID, "select name,id from genres");
			}
			else MessageBox.Show("No item selected");
		}
		//Existing row Deletion
		private void DeleteActor(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateActorLNFNID.Items)
			{
				exists = item == UpdateActorLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateActorLNFNID.Text = "null";
			
			if (UpdateActorLNFNID.Text != "null")
			{
				//combobox contains last_name first_name id
				string actor_id = UpdateActorLNFNID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from actors
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", actor_id);
					try
					{
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				//setquery("delete from actors where id=" + actor_id + ")");
				//refresh actors combobox
				ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
				ComboboxRefresh(UpdateActorLNFNID, "select last_name,first_name,id from actors");
			}
			else MessageBox.Show("No item selected");
		}
		private void DeleteDirector(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateDirectorLNFNID.Items)
			{
				exists = item == UpdateDirectorLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateDirectorLNFNID.Text = "null";

			if (UpdateDirectorLNFNID.Text != "null")
			{
				//combobox contains last_name first_name id
				string director_id = UpdateDirectorLNFNID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from directors
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", director_id);
					try
					{
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				//setquery("delete from directors where id=" + director_id + ")");
				//refresh directors combobox
				ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
				ComboboxRefresh(UpdateDirectorLNFNID, "select last_name,first_name,id from directors");
			}
			else MessageBox.Show("No item selected");
		}
		private void DeleteCountry(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateCountryNameID.Items)
			{
				exists = item == UpdateCountryNameID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateCountryNameID.Text = "null";

			if (UpdateCountryNameID.Text != "null")
			{
				//combobox contains name id
				string country_id = UpdateCountryNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from countries
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", country_id);
					try
					{
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				//setquery("delete from countries where id=" + country_id + ")");
				//refresh countries combobox
				ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
				ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");
			}
			else MessageBox.Show("No item selected");
		}
		private void DeleteLang(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateLangNameID.Items)
			{
				exists = item == UpdateLangNameID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateLangNameID.Text = "null";

			if (UpdateLangNameID.Text != "null")
			{
				//combobox contains name id
				string lang_id = UpdateLangNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from langs
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", lang_id);
					try
					{
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				//setquery("delete from langs where id=" + lang_id + ")");
				//refresh langs combobox
				ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
				ComboboxRefresh(UpdateLangNameID, "select name,id from langs");
			}
			else MessageBox.Show("No item selected");
		}
		private void DeleteFormat(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateFormatNameID.Items)
			{
				exists = item == UpdateFormatNameID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateFormatNameID.Text = "null";

			if (UpdateFormatNameID.Text != "null")
			{
				//combobox contains name id
				string format_id = UpdateFormatNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from formats
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", format_id);
					try
					{
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				//setquery("delete from formats where id=" + format_id + ")");
				//refresh formats combobox
				ComboboxRefresh(MovieFormFormatNameID, "select name,id from format");
				ComboboxRefresh(UpdateFormatNameID, "select name,id from format");
			}
			else MessageBox.Show("No item selected");
		}
		private void DeleteGenre(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in UpdateGenreNameID.Items)
			{
				exists = item == UpdateGenreNameID.Text;
				if (exists) break;
			}
			if (exists == false) UpdateGenreNameID.Text = "null";

			if (UpdateGenreNameID.Text != "null")
			{
				//combobox contains name id
				string genre_id = UpdateGenreNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from genres
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", genre_id);
					try
					{
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
				//setquery("delete from genres where id=" + genre_id + ")");
				//refresh genres combobox
				ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
				ComboboxRefresh(UpdateGenreNameID, "select name,id from genres");
			}
			else MessageBox.Show("No item selected");
		}
	}
}