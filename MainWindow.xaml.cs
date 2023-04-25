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
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Windows.Threading;
using xctk = Xceed.Wpf.Toolkit;

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
					"movies.total_count as 'all copies',movies.plot, " +
					"CASE " +
					"	WHEN poster_path IS NULL THEN 'no' " +
					"	WHEN poster_path IS NOT NULL THEN 'yes' END AS poster, " +
					"CASE " +
					"	WHEN poster_path IS NULL THEN 'no' " +
					"	WHEN trailer_path IS NOT NULL THEN 'yes' END AS trailer " +
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
				"CONCAT(movies.name,' ',movies.id) as movie,movies.year, " +
				"CASE WHEN orders.rent_date IS NULL THEN '' " +
				"	WHEN orders.rent_date IS NOT NULL THEN orders.rent_date END AS 'rent date', " +
				"CASE WHEN orders.due_date IS NULL THEN '' " +
				"	WHEN orders.due_date IS NOT NULL THEN orders.due_date END AS 'due date', " +
				"CASE WHEN orders.return_date IS NULL THEN '' " +
				"	WHEN orders.return_date IS NOT NULL THEN orders.return_date END AS 'return date' " +
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
			ComboboxRefresh(DeleteActorLNFNID, "select actors.last_name,actors.first_name,actors.id from actors left outer join movies on actors.id=movies.actor_id where movies.actor_id is null");
			ComboboxRefresh(UpdateActorLNFNID, "select last_name,first_name,id from actors");

			//Directors
			ComboboxRefresh(DeleteDirectorLNFNID, "select directors.last_name,directors.first_name,directors.id from directors left outer join movies on directors.id=movies.director_id where movies.director_id is null");
			ComboboxRefresh(UpdateDirectorLNFNID, "select last_name,first_name,id from directors");

			//Countries
			ComboboxRefresh(DeleteCountryNameID, "select countries.name,countries.id from countries left outer join movies on countries.id=movies.country_id where movies.country_id is null");
			ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");

			//Languages
			ComboboxRefresh(DeleteLangNameID, "select langs.name,langs.id from langs left outer join movies on langs.id=movies.lang_id where movies.lang_id is null");
			ComboboxRefresh(UpdateLangNameID, "select name,id from langs");

			//Formats
			ComboboxRefresh(DeleteFormatNameID, "select formats.name,formats.id from formats left outer join movies on formats.id=movies.format_id where movies.format_id is null");
			ComboboxRefresh(UpdateFormatNameID, "select name,id from formats");

			//Genres
			ComboboxRefresh(DeleteGenreNameID, "select genres.name,genres.id from genres left outer join movies on genres.id=movies.genre_id where movies.genre_id is null");
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
					xctk.MessageBox.Show("no rows affected");
				}
				else
				{
					//Affected
					xctk.MessageBox.Show("rows affected: " + a);
				}
			}
			catch (Exception ex)
			{
				//Not affected
				xctk.MessageBox.Show("no rows affected:\n" + ex.ToString());
			}
		}
		//for sql panel; select getquery or setquery based on select existence
		public void SQLquery()
		{
			if (SQLqueryString.Text != string.Empty)
			{
				if (SQLqueryString.Text.Contains("Select"))
				{
					setquery(SQLqueryString.Text);
				}
				else getquery(SQLgrid, SQLqueryString.Text);
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
			grid.ItemsSource = null;
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

			TotalTime = Trailer.NaturalDuration.TimeSpan;

			// Create a timer that will update the counters and the time slider
			DispatcherTimer timerVideoTime = new DispatcherTimer();
			timerVideoTime.Interval = TimeSpan.FromSeconds(1);
			timerVideoTime.Tick += new EventHandler(timer_Tick);
			timerVideoTime.Start();
		}
		private TimeSpan TotalTime;
		public void timer_Tick(object sender, EventArgs e)
		{
			// Check if the movie calculated its total time
			if (Trailer.NaturalDuration.TimeSpan.TotalSeconds > 0)
			{
				if (TotalTime.TotalSeconds > 0)
				{
					// Updating time slider
					SeekSlider.Value = Trailer.Position.TotalSeconds / TotalTime.TotalSeconds;
				}
			}
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
			if (MovieTrailerID.Text != "ID")
			{
				//set movie info in poster panel
				DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
				MovieTrailerID.Text = rowview.Row["id"].ToString();
				MovieTrailerTitle.Text = rowview.Row["title"].ToString();
				MovieTrailerYear.Text = rowview.Row["year"].ToString();

				//get path to file if value present
				string exists = rowview.Row["trailer"].ToString();
				string trailer_path = getquery("select trailer_path from movies where id=" + MovieTrailerID.Text);
				if (exists == "yes" && File.Exists(trailer_path))
				{
					//Uri uri = new Uri(trailer_path);
					//Trailer.Source = uri;
					Trailer.Source = new Uri(trailer_path);
					xctk.MessageBox.Show("Trailer set in trailer tab");
				}
				else if (exists == "yes" && !File.Exists(trailer_path))
				{
					xctk.MessageBox.Show("Trailer not found at set location; it's path will be removed. Set new path from trailer tab");
					//Remove outdated trailer_path
					RemoveTrailer(null, null);
					MoviesGridRefresh();
				}
				else if (exists == "no") xctk.MessageBox.Show("Trailer for this movie is not set. Set new it in trailer tab");
			}
			else xctk.MessageBox.Show("Load trailer from movies catalog");
		}
		//Set trailer file path and update database
		private void SetTrailer(object sender, MouseButtonEventArgs e)
		{
			if (MovieTrailerID.Text != "ID")
			{
				OpenFileDialog op = new OpenFileDialog();
				op.Title = "Select trailer";
				if (op.ShowDialog() == true)
				{
					//Uri uri = new Uri(op.FileName);
					//Trailer.Source = uri;
					Trailer.Source = new Uri(op.FileName);
					//update trailer_path in database
					using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
					{
						connection.Open();
						var sql = @"UPDATE movies SET trailer_path=@trailer_path WHERE id=@id";
						var cmd = new SqlCommand(sql, connection);
						cmd.Parameters.AddWithValue("@id", MovieTrailerID.Text);
						cmd.Parameters.AddWithValue("@trailer_path", op.FileName);
						string result = cmd.ExecuteNonQuery().ToString();
						if (result == "1")
						{
							xctk.MessageBox.Show("Trailer set");
							MoviesGridRefresh();
						}
						else if (result == "0") xctk.MessageBox.Show("Failed to set trailer");
					}
					//setquery("update movies set trailer_path=" + uri + " where id=" + getmovie_id());
				}
			}
			else xctk.MessageBox.Show("Load trailer from movies catalog");
		}
		private void RemoveTrailer(object sender, MouseButtonEventArgs e)
		{
			if (MovieTrailerID.Text != "ID")
			{
				Trailer.Source = null;
				//update database
				setquery("update movies set trailer_path=null where id=" + MovieTrailerID.Text);
			}
			else xctk.MessageBox.Show("Load trailer from movies catalog first");
		}
		//Poster panel buttons
		private void RemovePoster(object sender, RoutedEventArgs e)
		{
			if (MoviePosterID.Text != "ID")
			{
				Poster.Source = null;
				//remove movie poster from database
				setquery("update movies set poster_path=null where id=" + MoviePosterID.Text);
				MoviesGridRefresh();
			}
			else xctk.MessageBox.Show("Load poster from movies catalog first");
		}
		private void SetPoster(object sender, MouseButtonEventArgs e)
		{
			if (MoviePosterID.Text != "ID")
			{
				OpenFileDialog op = new OpenFileDialog();
				op.Title = "Select poster";
				op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
				  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
				  "Portable Network Graphic (*.png)|*.png";
				if (op.ShowDialog() == true)
				{
					//load poster from file path
					//Uri uri = new Uri(op.FileName);
					//BitmapImage img = new BitmapImage(uri);
					//Poster.Source = img;
					Poster.Source = new BitmapImage(new Uri(op.FileName));
					//add poster_path to database
					using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
					{
						connection.Open();
						var sql = @"UPDATE movies SET poster_path=@poster_path WHERE id=@id";
						var cmd = new SqlCommand(sql, connection);
						cmd.Parameters.AddWithValue("@id", MoviePosterID.Text);
						cmd.Parameters.AddWithValue("@poster_path", op.FileName);
						string result = cmd.ExecuteNonQuery().ToString();
						if (result == "1")
						{
							xctk.MessageBox.Show("Poster set");
							MoviesGridRefresh();
						}
						else if (result == "0") xctk.MessageBox.Show("Failed to set poster");
					}
					MoviesGridRefresh();
				}
			}
			else xctk.MessageBox.Show("Load poster from movies catalog");
		}
		//load poster if exists
		private void LoadPoster(object sender, RoutedEventArgs e)
		{
			if (MoviePosterID.Text != "ID")
			{
				Poster.Source = null;

				//set movie info in poster panel
				DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
				MoviePosterID.Text = rowview.Row["id"].ToString();
				MoviePosterTitle.Text = rowview.Row["title"].ToString();
				MoviePosterYear.Text = rowview.Row["year"].ToString();

				//get path to file if value present
				string poster_path = getquery("select poster_path from movies where id=" + getmovie_id());
				string exists = rowview.Row["poster"].ToString();
				//poster set but file not found
				if (exists == "yes" && File.Exists(poster_path))
				{
					//poster_path = "pack://application:,,,/" + poster_path;
					//Uri uri = new Uri(poster_path);
					//BitmapImage img = new BitmapImage(uri);
					//Poster.Source = img;
					Poster.Source = new BitmapImage(new Uri(poster_path));
					xctk.MessageBox.Show("poster set in poster tab");
				}
				//trailer set but file not found
				else if (exists == "yes" && !File.Exists(poster_path))
				{
					xctk.MessageBox.Show("Poster not found at it's set location. It's path will be removed. Set new file path from poster tab");
					//Remove outdated trailer_path
					RemovePoster(null, null);
					MoviesGridRefresh();
				}
				//no poster path in database
				else if (exists == "no") xctk.MessageBox.Show("Poster for this movie is not set. You can set it in poster tab.");
			}
			else xctk.MessageBox.Show("Load poster from movies catalog");
		}
		//Submit Buttons
		private void SubmitOrder(object sender, MouseButtonEventArgs e)
		{
			//check if client exists (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in OrderFormClientLNFNID.Items)
			{
				exists = item == OrderFormClientLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) OrderFormClientLNFNID.Text = "null";

			//Proceed on client and movie set
			if (OrderFormClientLNFNID.Text != "null" && OrderFormMovieID.Text != "")
			{
				//get number of copies left available
				int copies_left = 0;
				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"select left_count from movies where id=@id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", OrderFormMovieID.Text);
					copies_left = int.Parse(cmd.ExecuteScalar().ToString());
				}
				string movie_id = OrderFormClientLNFNID.Text.Split().Last();
				//add mode if copies available
				if (mode == false && copies_left > 0)
				{
							using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
							{
								connection.Open();
								var sql = @"Insert into Orders (client_id,movie_id,rent_date,due_date,return_date) " +
									"values(@client_id,@movie_id," +
									"@rent_date,@due_date,@return_date)";
								using (var cmd = new SqlCommand(sql, connection))
								{
									cmd.Parameters.AddWithValue("@client_id", movie_id);
									cmd.Parameters.AddWithValue("@movie_id", OrderFormMovieID.Text);
									cmd.Parameters.AddWithValue("@rent_date", OrderFormRentDate.Text);
									cmd.Parameters.AddWithValue("@due_date", OrderFormDueDate.Text);
									cmd.Parameters.AddWithValue("@return_date", OrderFormReturnDate.Text);
									xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
									OrdersGridRefresh();
									//decrement movie copies available when client rents a copy
									copies_left--;
									OrderClearForm();
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
				//add mode no copies left
				else if (mode == false && copies_left <= 0) xctk.MessageBox.Show("No copies left to rent");
				//edit mode
				else if (mode == true && OrderFormID.Text != "")
				{
							using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
							{
								connection.Open();
								var sql = @"update orders set client_id=@client_id, movie_id=@movie_id," +
									"rent_date=@rent_date, due_date=@due_date, return_date=@return_date " +
									"where id=@id";
								using (var cmd = new SqlCommand(sql, connection))
								{
									cmd.Parameters.AddWithValue("@id", OrderFormID.Text);
									cmd.Parameters.AddWithValue("@client_id", OrderFormClientLNFNID.Text);
									cmd.Parameters.AddWithValue("@movie_id", OrderFormMovieID.Text);
									cmd.Parameters.AddWithValue("@rent_date", OrderFormRentDate.Text);
									cmd.Parameters.AddWithValue("@due_date", OrderFormDueDate.Text);
									cmd.Parameters.AddWithValue("@return_date", OrderFormReturnDate.Text);
									xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
									OrdersGridRefresh();
								}
							}
				}else if (mode == true && OrderFormID.Text == "") xctk.MessageBox.Show("Order id not set. Use edit context menu option in orders catalog");
			}else xctk.MessageBox.Show("Fill client and movie");
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

				//add mode when names set
				if (mode == false && ClientFormFirstName.Text != "" && ClientFormLastName.Text != "")
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
								xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
								//refresh grid
								ClientsGridRefresh();
								ClientsComboboxRefresh();
								ClientClearForm();
							}
						}
				} else if (mode == false && ClientFormFirstName.Text == "" && ClientFormLastName.Text == "") xctk.MessageBox.Show("First name and last name must be filled correctly");
			//edit mode
			else if (mode == true && ClientFormID.Text != "")
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
							xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
							//refresh grid
							ClientsGridRefresh();
							ClientsComboboxRefresh();
						}
					}
				}
				else xctk.MessageBox.Show("To set client use edit context menu option in clients catalog");
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
			if (mode == false && MovieFormTitle.Text != "")
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
							xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
							MoviesGridRefresh();
							MovieClearForm();
						}
					}
			}
			//add mode no title set
			else if (mode == false && MovieFormTitle.Text == "") xctk.MessageBox.Show("Movie title not set");
			//edit mode
			else if (mode = true && MovieFormID.Text != "")
			{
					using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
					{
						connection.Open();
						var sql = @"update movies set name=@name,year=@year,country_id=@country_id,duration=@duration,age=@age," +
							"total_count=@total_count,price=@price,left_count=@left_count,plot=@plot,lang_id=@lang_id," +
							"actor_id=@actor_id,director_id=@director_id,format_id=@format_id,genre_id=@genre_id " +
							"where id=@id";
						using (var cmd = new SqlCommand(sql, connection))
						{
							cmd.Parameters.AddWithValue("@id", MovieFormID.Text);
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
							xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
							MoviesGridRefresh();
						}
					}
			}else xctk.MessageBox.Show("Movie id not set. Use edit context menu option from movies catalog");
		}
		//Movies right click menu
		private void MovieItem_plot(object sender, RoutedEventArgs e)
		{
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
			string plot = rowview.Row["plot"].ToString();
			xctk.MessageBox.Show(plot);
		}
		//Fills existing movie data into form in admin panel (rent movie option)
		//Clear movie form
		public void MovieClearForm() 
		{
			MovieFormID.Text = null;
			MovieFormTitle.Text = null;
			MovieFormYear.Text = null;
			MovieFormDuration.Text = null;
			MovieFormAge.Text = null;
			MovieFormPrice.Text = null;
			MovieFormCopiesLeft.Text = null;
			MovieFormCopiesTotal.Text = null;
			MovieFormPlot.Text = "Initial Premise";

			//comboboxes
			MovieFormGenreNameID.Text = "null";
			MovieFormFormatNameID.Text = "null";
			MovieFormDirectorLNFNID.Text = "null";
			MovieFormActorLNFNID.Text = "null";
			MovieFormCountryNameID.Text = "null";
			MovieFormLangNameID.Text = "null";
		}
		//Clear client form
		public void ClientClearForm()
		{
			ClientFormID.Text = null;
			ClientFormLastName.Text = null;
			ClientFormFirstName.Text = null;
			ClientFormEmail.Text = null;
			ClientFormPhone.Text = null;
		}
		//Clear order form
		public void OrderClearForm()
		{
			OrderFormID.Text = null;
			OrderFormMovieID.Text = null;
			OrderFormClientLNFNID.Text = null;
			//dates
			OrderFormRentDate.Text = null;
			OrderFormDueDate.Text = null;
			OrderFormReturnDate.Text = null;
		}
		private void MovieItem_rent(object sender, RoutedEventArgs e)
		{
			//add mode
			SelectAddMode(null,null);
			//autofill form
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;

			OrderFormID.Text = null;
			OrderFormMovieID.Text = rowview.Row["id"].ToString();

			//combobox
			OrderFormClientLNFNID.Text = null;
			
			//dates
			OrderFormRentDate.Text = null;
			OrderFormDueDate.Text = null;
			OrderFormReturnDate.Text = null;

			xctk.MessageBox.Show("Movie data set. Submit order in admin panel.");
		}
		private void MovieItem_edit(object sender, RoutedEventArgs e)
		{
			MovieClearForm();
			//edit mode
			SelectEditMode(null, null);
			
			//autofill form
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;

			MovieFormID.Text = rowview.Row["id"].ToString();
			MovieFormTitle.Text = rowview.Row["title"].ToString();
			MovieFormYear.Text = rowview.Row["year"].ToString();
			MovieFormDuration.Text = rowview.Row["duration"].ToString();
			MovieFormAge.Text = rowview.Row["age"].ToString();
			MovieFormPrice.Text = rowview.Row["price"].ToString();
			MovieFormCopiesLeft.Text = rowview.Row["left copies"].ToString();
			MovieFormCopiesTotal.Text = rowview.Row["all copies"].ToString();
			MovieFormPlot.Text = rowview.Row["plot"].ToString();

			//comboboxes
			MovieFormGenreNameID.Text = rowview.Row["genre"].ToString();
			MovieFormFormatNameID.Text = rowview.Row["format"].ToString();
			MovieFormDirectorLNFNID.Text = rowview.Row["director"].ToString();
			MovieFormActorLNFNID.Text = rowview.Row["lead actor"].ToString();
			MovieFormCountryNameID.Text = rowview.Row["country"].ToString();
			MovieFormLangNameID.Text = rowview.Row["language"].ToString();

			//xctk.MessageBox.Show("Movie data set. Edit and submit changes in admin panel.");
			xctk. MessageBox.Show("Movie data set. Edit and submit changes in admin panel.");
		}
		private void MovieItem_delete(object sender, RoutedEventArgs e)
		{
			//clear poster and trailer if set
			if (int.Parse(MovieTrailerID.Text) == getmovie_id())
			{
				MovieTrailerID.Text = "ID";
				MovieTrailerTitle.Text = "Title";
				MovieTrailerYear.Text = "Year";
				Trailer.Source = null;
			}
			if (int.Parse(MoviePosterID.Text) == getmovie_id())
			{
				MoviePosterID.Text = "ID";
				MoviePosterTitle.Text = "Title";
				MoviePosterYear.Text = "Year";
				Poster.Source = null;
			}
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
						xctk.MessageBox.Show(ex.Message);
					}
				}
				MoviesGridRefresh();
		}
		private void MovieItem_trailer(object sender, RoutedEventArgs e)
		{
			TrailerInit(null, null);
		}
		//Clients right click menu
		//Fills existing client data into form in admin panel (edit client option)
		private void ClientItem_edit(object sender, RoutedEventArgs e)
		{
			ClientClearForm();
			//edit mode
			SelectEditMode(null, null);
			
			//fill form
			ClientFormID.Text = getclient_id().ToString();
			DataRowView rowview = ClientsCatalog.SelectedItem as DataRowView;
			ClientFormLastName.Text = rowview.Row["last name"].ToString();
			ClientFormFirstName.Text = rowview.Row["first name"].ToString();
			ClientFormEmail.Text = rowview.Row["email"].ToString();
			ClientFormPhone.Text = rowview.Row["phone"].ToString();

			xctk.MessageBox.Show("Client data set. Submit in admin panel.");
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
					xctk.MessageBox.Show(ex.Message);
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
				xctk.MessageBox.Show("Movie already returned");
			}
		}
		//Fills existing order data into form in admin panel (edit order option)
		private void OrderItem_edit(object sender, RoutedEventArgs e)
		{
			OrderClearForm();
			//edit mode
			SelectEditMode(null, null);
			
			//fill form
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
			
			//get due date
			string due_date = rowview.Row["due date"].ToString();
			OrderFormDueDate.Text = due_date;
			
			//get return date
			string return_date = rowview.Row["return date"].ToString();
			OrderFormReturnDate.Text = return_date;
			
			xctk.MessageBox.Show("Order data set. Edit order in admin panel.");
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
				movies_dt.DefaultView.RowFilter = "title like '%" + FilterMovieTitleString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMoviePrice(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "price like '%" + FilterMoviePriceString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieAge(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "age like '%" + FilterMovieAgeString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieDuration(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "duration like '%" + FilterMovieDurationString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieGenre(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "genre like '%" + FilterMovieGenreString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieYear(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "year like '%" + FilterMovieYearString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieCopiesTotal(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "'copies total' like '%" + FilterMovieCopiesTotalString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieCopiesLeft(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "'copies left' like '%" + FilterMovieCopiesLeftString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieCountry(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "country like '%" + FilterMovieCountryString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieLang(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "language like '%" + FilterMovieLangString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieFormat(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "format like '%" + FilterMovieFormatString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieDirector(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "director like '%" + FilterMovieDirectorString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterMovieActor(object sender, TextChangedEventArgs e)
		{
			try
			{
				movies_dt.DefaultView.RowFilter = "'lead actor' like '%" + FilterMovieActorString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		//Filter clients
		private void FilterClientLastName(object sender, TextChangedEventArgs e)
		{
			try
			{
				clients_dt.DefaultView.RowFilter = "'last name' like '%" + FilterClientLastNameString.Text + "%'";
				//ClientsGridRefresh();
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
			/*
			using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
			{
				connection.Open();
				var sql = @"Select id, last_name as 'last name', first_name as 'first name', phone, email from clients
							where last_name like '%@last_name%'";// and first_name like '%@first_name%' and phone like '%@phone%' and email like '%@email%'";
				var cmd = new SqlCommand(sql, connection);
				cmd.Parameters.AddWithValue("@first_name", FilterClientFirstNameString.Text);
				cmd.Parameters.AddWithValue("@last_name", FilterClientLastNameString.Text);
				cmd.Parameters.AddWithValue("@phone", FilterClientPhoneString.Text);
				cmd.Parameters.AddWithValue("@email", FilterClientEmailString.Text);
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
					xctk.MessageBox.Show(ex.Message);
				}
			}
			*/
			//DataView dataView = clients_dt.DefaultView;
			//if (!string.IsNullOrEmpty(FilterClientLastNameString.Text))
			//{
			//	dataView.RowFilter = "'last name' = " + FilterClientLastNameString.Text;
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
				"where clients.last_name like '@FilterClientLastNameString.Text%'");

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
				xctk.MessageBox.Show(ex.Message);
			}
			*/
		}
		private void FilterClientFirstName(object sender, TextChangedEventArgs e)
		{
			try
			{
				clients_dt.DefaultView.RowFilter = "'first name' like '%" + FilterClientFirstNameString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
			
		}
		private void FilterClientEmail(object sender, TextChangedEventArgs e)
		{
			try
			{
				clients_dt.DefaultView.RowFilter = "'email' like '%" + FilterClientEmailString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
			
		}
		private void FilterClientPhone(object sender, TextChangedEventArgs e)
		{
			try
			{
				clients_dt.DefaultView.RowFilter = "'phone' like '%" + FilterClientPhoneString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		//Filter orders
		private void FilterOrderMovie(object sender, TextChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'movie' like '%" + FilterOrderMovieString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderClient(object sender, TextChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'client' like '%" + FilterOrderClientString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		
		private void FilterOrderYear(object sender, TextChangedEventArgs e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'year' like '%" + FilterOrderYearString.Text + "%'";
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderRentStartDate(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'rent date' >= " + FilterOrderRentStartDateString.Text;
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderRentStopDate(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'rent date' <= " + FilterOrderRentStopDateString.Text;
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderDueStartDate(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'due date' >= " + FilterOrderDueStartDateString.Text;
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderDueStopDate(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'due date' <= " + FilterOrderDueStopDateString.Text;
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderReturnStartDate(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'return date' >= " + FilterOrderReturnStartDateString.Text;
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
			}
		}
		private void FilterOrderReturnStopDate(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			try
			{
				orders_dt.DefaultView.RowFilter = "'return date' <= " + FilterOrderReturnStopDateString.Text;
			}
			catch (Exception ex)
			{
				xctk.MessageBox.Show(ex.Message);
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
			xctk.MessageBox.Show("Available tables:\n" +
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into actors (first_name,last_name) values(" + AddActorFirstName.Text + "," + AddActorLastName.Text +")");
			//refresh actors combobox
			ComboboxRefresh(UpdateActorLNFNID, "select last_name,first_name,id from actors");
			ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
			ComboboxRefresh(DeleteActorLNFNID, "select actors.last_name,actors.first_name,actors.id from actors left outer join movies on actors.id=movies.actor_id where movies.actor_id is null");
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into directors (first_name,last_name) values(" + AddActorFirstName.Text + "," + AddActorLastName.Text + ")");
			//refresh directors combobox
			ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
			ComboboxRefresh(UpdateDirectorLNFNID, "select last_name,first_name,id from directors");
			ComboboxRefresh(DeleteDirectorLNFNID, "select directors.last_name,directors.first_name,directors.id from directors left outer join movies on directors.id=movies.director_id where movies.director_id is null");
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into countries (name) values(" + AddCountryName.Text + ")");
			//refresh countries combobox
			ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
			ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");
			ComboboxRefresh(DeleteCountryNameID, "select countries.name,countries.id from countries left outer join movies on countries.id=movies.country_id where movies.country_id is null");
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into langs (name) values(" + AddLangName.Text + ")");
			//refresh langs combobox
			ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
			ComboboxRefresh(UpdateLangNameID, "select name,id from langs");
			ComboboxRefresh(DeleteLangNameID, "select langs.name,langs.id from langs left outer join movies on langs.id=movies.lang_id where movies.lang_id is null");
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into formats (name) values(" + AddFormatName.Text + ")");
			//refresh formats combobox
			ComboboxRefresh(MovieFormFormatNameID, "select name,id from formats");
			ComboboxRefresh(UpdateFormatNameID, "select name,id from formats");
			ComboboxRefresh(DeleteFormatNameID, "select formats.name,formats.id from formats left outer join movies on formats.id=movies.format_id where movies.format_id is null");
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
			}
			//setquery("Insert into genres (name) values(" + AddGenreName.Text + ")");
			//refresh genres combobox
			ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
			ComboboxRefresh(UpdateGenreNameID, "select name,id from genres");
			ComboboxRefresh(DeleteGenreNameID, "select genres.name,genres.id from genres left outer join movies on genres.id=movies.genre_id where movies.genre_id is null");
		}
		//Existing row updating
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//refresh actors combobox
				ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
				ComboboxRefresh(UpdateActorLNFNID, "select last_name,first_name,id from actors");
				ComboboxRefresh(DeleteActorLNFNID, "select actors.last_name,actors.first_name,actors.id from actors left outer join movies on actors.id=movies.actor_id where movies.actor_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//refresh directors combobox
				ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
				ComboboxRefresh(UpdateDirectorLNFNID, "select last_name,first_name,id from directors");
				ComboboxRefresh(DeleteDirectorLNFNID, "select directors.last_name,directors.first_name,directors.id from directors left outer join movies on directors.id=movies.director_id where movies.director_id is null");

			}
			else xctk.MessageBox.Show("No item selected");
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//refresh countries combobox
				ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
				ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");
				ComboboxRefresh(DeleteCountryNameID, "select countries.name,countries.id from countries left outer join movies on countries.id=movies.country_id where movies.country_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//setquery("update langs set name=" + UpdateLangName.Text + "where id=" + lang_id + ")");
				//refresh langs combobox
				ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
				ComboboxRefresh(UpdateLangNameID, "select name,id from langs");
				ComboboxRefresh(DeleteLangNameID, "select langs.name,langs.id from langs left outer join movies on langs.id=movies.lang_id where movies.lang_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//setquery("update formats set name=" + UpdateFormatName.Text + "where id=" + format_id + ")");
				//refresh formats combobox
				ComboboxRefresh(MovieFormFormatNameID, "select name,id from formats");
				ComboboxRefresh(UpdateFormatNameID, "select name,id from formats");
				ComboboxRefresh(DeleteFormatNameID, "select formats.name,formats.id from formats left outer join movies on formats.id=movies.format_id where movies.format_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
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
					xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
				}
				//setquery("update genres set name=" + UpdateGenreName.Text + "where id=" + genre_id + ")");
				//refresh genres combobox
				ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
				ComboboxRefresh(UpdateGenreNameID, "select name,id from genres");
				ComboboxRefresh(DeleteGenreNameID, "select genres.name,genres.id from genres left outer join movies on genres.id=movies.genre_id where movies.genre_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
		}
		//Existing row Deletion
		private void DeleteActor(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in DeleteActorLNFNID.Items)
			{
				exists = item == DeleteActorLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) DeleteActorLNFNID.Text = "null";
			
			if (DeleteActorLNFNID.Text != "null")
			{
				//combobox contains last_name first_name id
				string actor_id = DeleteActorLNFNID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from actors
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", actor_id);
					try
					{
						xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						xctk.MessageBox.Show(ex.Message);
					}
				}
				//refresh actors combobox
				ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
				ComboboxRefresh(UpdateActorLNFNID, "select last_name,first_name,id from actors");
				ComboboxRefresh(DeleteActorLNFNID, "select actors.last_name,actors.first_name,actors.id from actors left outer join movies on actors.id=movies.actor_id where movies.actor_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
		}
		private void DeleteDirector(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in DeleteDirectorLNFNID.Items)
			{
				exists = item == DeleteDirectorLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) DeleteDirectorLNFNID.Text = "null";

			if (DeleteDirectorLNFNID.Text != "null")
			{
				//combobox contains last_name first_name id
				string director_id = DeleteDirectorLNFNID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from directors
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", director_id);
					try
					{
						xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						xctk.MessageBox.Show(ex.Message);
					}
				}
				//setquery("delete from directors where id=" + director_id + ")");
				//refresh directors combobox
				ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
				ComboboxRefresh(UpdateDirectorLNFNID, "select last_name,first_name,id from directors");
				ComboboxRefresh(DeleteDirectorLNFNID, "select directors.last_name,directors.first_name,directors.id from directors left outer join movies on directors.id=movies.director_id where movies.director_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
		}
		private void DeleteCountry(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in DeleteCountryNameID.Items)
			{
				exists = item == DeleteCountryNameID.Text;
				if (exists) break;
			}
			if (exists == false) DeleteCountryNameID.Text = "null";

			if (DeleteCountryNameID.Text != "null")
			{
				//combobox contains name id
				string country_id = DeleteCountryNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from countries
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", country_id);
					try
					{
						xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						xctk.MessageBox.Show(ex.Message);
					}
				}
				//refresh countries combobox
				ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
				ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");
				ComboboxRefresh(DeleteCountryNameID, "select countries.name,countries.id from countries left outer join movies on countries.id=movies.country_id where movies.country_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
		}
		private void DeleteLang(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in DeleteLangNameID.Items)
			{
				exists = item == DeleteLangNameID.Text;
				if (exists) break;
			}
			if (exists == false) DeleteLangNameID.Text = "null";

			if (DeleteLangNameID.Text != "null")
			{
				//combobox contains name id
				string lang_id = DeleteLangNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from langs
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", lang_id);
					try
					{
						xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						xctk.MessageBox.Show(ex.Message);
					}
				}
				//refresh langs combobox
				ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
				ComboboxRefresh(UpdateLangNameID, "select name,id from langs");
				ComboboxRefresh(DeleteLangNameID, "select langs.name,langs.id from langs left outer join movies on langs.id=movies.lang_id where movies.lang_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
		}
		private void DeleteFormat(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in DeleteFormatNameID.Items)
			{
				exists = item == DeleteFormatNameID.Text;
				if (exists) break;
			}
			if (exists == false) DeleteFormatNameID.Text = "null";

			if (DeleteFormatNameID.Text != "null")
			{
				//combobox contains name id
				string format_id = DeleteFormatNameID.Text.Split(' ').Last();

				using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
				{
					connection.Open();
					var sql = @"delete from formats
						WHERE id = @id";
					var cmd = new SqlCommand(sql, connection);
					cmd.Parameters.AddWithValue("@id", format_id);
					try
					{
						xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						xctk.MessageBox.Show(ex.Message);
					}
				}
				//setquery("delete from formats where id=" + format_id + ")");
				//refresh formats combobox
				ComboboxRefresh(MovieFormFormatNameID, "select name,id from formats");
				ComboboxRefresh(UpdateFormatNameID, "select name,id from formats");
				ComboboxRefresh(DeleteFormatNameID, "select formats.name,formats.id from formats left outer join movies on formats.id=movies.format_id where movies.format_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
		}
		private void DeleteGenre(object sender, MouseButtonEventArgs e)
		{
			//remove bad values
			//check if combobox item exists in collection (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in DeleteGenreNameID.Items)
			{
				exists = item == DeleteGenreNameID.Text;
				if (exists) break;
			}
			if (exists == false) DeleteGenreNameID.Text = "null";

			if (DeleteGenreNameID.Text != "null")
			{
				//combobox contains name id
				string genre_id = DeleteGenreNameID.Text.Split(' ').Last();

					using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
					{
						connection.Open();
						var sql = @"delete from genres
						WHERE id = @id";
						var cmd = new SqlCommand(sql, connection);
						cmd.Parameters.AddWithValue("@id", genre_id);
						try
						{
							xctk.MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
						}
						catch (Exception ex)
						{
							xctk.MessageBox.Show(ex.Message);
						}
					}
					//refresh genres combobox
					ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
					ComboboxRefresh(UpdateGenreNameID, "select name,id from genres");
					ComboboxRefresh(DeleteGenreNameID, "select genres.name,genres.id from genres left outer join movies on genres.id=movies.genre_id where movies.genre_id is null");
			}
			else xctk.MessageBox.Show("No item selected");
		}
	}
}