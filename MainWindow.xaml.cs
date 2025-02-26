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
using System.Collections;
using System.Runtime.Remoting.Channels;

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
			MoviesGridRefresh(null,null);
			ClientsGridRefresh(null, null);
			OrdersGridRefresh(null, null);
			//Fill form comboboxes
			MoviesComboboxRefresh();
			ClientsComboboxRefresh();

			//Attributes Panel Comboboxes
			AttributesComboboxesRefresh();

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

			//disable order return date
			OrderFormReturnDate.IsEnabled = false;

			MovieClearForm();
			ClientClearForm();
			OrderClearForm();
		}
		DataTable movies_dt = new DataTable();
		DataTable clients_dt = new DataTable();
		DataTable orders_dt = new DataTable();
		void Reset_msg_error(object sender, MouseButtonEventArgs e) {
			Error.Text = "";
			MessageBar.Text = "";
		}
        //Grid Refreshes
        public void MoviesGridRefresh(object sender, EventArgs e) {
			string query = "Select " +
                    "movies.id as ID, " +
                    "movies.name as Title, movies.year as Year, movies.duration as Duration, movies.age as Age," +
                    "CASE WHEN genres.id <> 1 THEN CONCAT(genres.name,' ',genres.id) END AS Genre," +
                    "movies.price as Price," +
                    "CASE WHEN formats.id <> 1 THEN CONCAT(formats.name,' ',formats.id) END AS Format," +
                    "CASE WHEN directors.id <> 1 THEN CONCAT(directors.last_name,' ',directors.first_name,' ',directors.id) END AS Director," +
                    "CASE WHEN actors.id <> 1 THEN CONCAT(actors.last_name,' ',actors.first_name,' ',actors.id) END AS 'Lead actor'," +
                    "CASE WHEN countries.id <> 1 THEN CONCAT(countries.name,' ',countries.id) END AS Country," +
                    "CASE WHEN langs.id <> 1 THEN CONCAT(langs.name,' ',langs.id) END AS Language," +
                    "movies.left_count as 'Left copies'," +
                    "movies.total_count as 'All copies'" +
                    //"movies.plot as Plot, " +
                    //"CASE " +
                    //"	WHEN poster_path IS NULL THEN 'No' " +
                    //"	WHEN poster_path IS NOT NULL THEN 'Yes' END AS Poster " +
                    //"CASE " +
                    //"	WHEN poster_path IS NULL THEN 'no' " +
                    //"	WHEN trailer_path IS NOT NULL THEN 'yes' END AS trailer " +
                    "from movies " +
                    "join actors on actors.id = movies.actor_id " +
                    "join countries on countries.id = movies.country_id " +
                    "join langs on langs.id = movies.lang_id " +
                    "join directors on directors.id = movies.director_id " +
                    "join formats on formats.id = movies.format_id " +
                    "join genres on genres.id = movies.genre_id";
           
				//Get list of filters and append to query with ANDs inbetween
				var Filters = new ArrayList();

				//Filter by Title 
				if (!FilterMovieTitleString.Text.Equals("")) 
				{Filters.Add(" movies.name LIKE \'%" + FilterMovieTitleString.Text + "%\'");}
                
				//Filter by Year 
                if (!FilterMovieYearString.Text.Equals(""))
                {Filters.Add(" movies.year LIKE \'%" + FilterMovieYearString.Text + "%\'");}
                
				//Filter by Genre 
                if (!FilterMovieGenreString.Text.Equals(""))
                {Filters.Add(" genres.name LIKE \'%" + FilterMovieGenreString.Text + "%\'");}
                
				//Filter by Age 
                if (!FilterMovieAgeString.Text.Equals(""))
                {Filters.Add(" movies.age LIKE \'%" + FilterMovieAgeString.Text + "%\'");}
                
				//Filter by Actor 
                if (!FilterMovieActorString.Text.Equals(""))
                {Filters.Add(" actors.last_name LIKE \'%" + FilterMovieActorString.Text + "%\'");}
                
				//Filter by Director 
                if (!FilterMovieDirectorString.Text.Equals(""))
                {Filters.Add(" directors.last_name LIKE \'%" + FilterMovieDirectorString.Text + "%\'");}
                
				//Filter by Format 
                if (!FilterMovieFormatString.Text.Equals(""))
                {Filters.Add(" formats.name LIKE \'%" + FilterMovieFormatString.Text + "%\'");}
                
				//Filter by Lang 
                if (!FilterMovieLangString.Text.Equals(""))
                {Filters.Add(" langs.name LIKE \'%" + FilterMovieLangString.Text + "%\'");}

				//Filter by Country 
				if (!FilterMovieCountryString.Text.Equals(""))
				{ Filters.Add(" countries.name LIKE \'%" + FilterMovieCountryString.Text + "%\'"); }


            //If filtering enabled add filters to query
            if (Filters.Count > 0) { query += " WHERE ";

				if (Filters.Count > 1)
					for (int i = 0; i < Filters.Count; i++)
					{
						query += Filters[i];
						if (i < Filters.Count - 1) { query += " AND "; }
					}
				else { query += Filters[0]; }
            }
            
			//query
			try
            {
                getquery(movies_dt, query);
            }
            catch (Exception ex)
            {

                Error.Text = ex.Message;
            }
		}
		public void ClientsGridRefresh(object sender, EventArgs e)
		{
			string query = "Select " +
				"id as ID," +
				"last_name as 'Last name', first_name as 'First name', phone as Phone, email as Email from clients";
            
            //Get list of filters and append to query with ANDs inbetween
            var Filters = new ArrayList();

            //Filter by Last Name 
            if (!FilterClientLastNameString.Text.Equals(""))
            { Filters.Add(" clients.last_name LIKE \'%" + FilterClientLastNameString.Text + "%\'"); }

            //Filter by First Name 
            if (!FilterClientFirstNameString.Text.Equals(""))
            { Filters.Add(" clients.first_name LIKE \'%" + FilterClientFirstNameString.Text + "%\'"); }

            //Filter by Phone 
            if (!FilterClientPhoneString.Text.Equals(""))
            { Filters.Add(" clients.phone LIKE \'%" + FilterClientPhoneString.Text + "%\'"); }

            //Filter by Email 
            if (!FilterClientEmailString.Text.Equals(""))
            { Filters.Add(" clients.email LIKE \'%" + FilterClientEmailString.Text + "%\'"); }

            //If filtering enabled add filters to query
            if (Filters.Count > 0)
            {
                query += " WHERE ";

                if (Filters.Count > 1)
                    for (int i = 0; i < Filters.Count; i++)
                    {
                        query += Filters[i];
                        if (i < Filters.Count - 1) { query += " AND "; }
                    }
                else { query += Filters[0]; }
            }

            //query
            try
            {
                getquery(clients_dt, query);
            }
            catch (Exception ex)
            {
                Error.Text = ex.Message;
            }
        }
		public void OrdersGridRefresh(object sender, EventArgs e)
		{
			string query = "Select " +
				"orders.id as ID," +
				"CONCAT(clients.last_name,' ',clients.first_name,' ',clients.id) as Client," +
				"CONCAT(movies.name,' ',movies.id) as Movie, movies.year as Year, " +
				"CASE WHEN orders.rent_date IS NOT NULL THEN FORMAT(orders.rent_date,'dd-MM-yyyy') END AS 'Rent date', " +
				"CASE WHEN orders.due_date IS NOT NULL THEN FORMAT(orders.due_date,'dd-MM-yyyy') END AS 'Due date', " +
				"CASE WHEN orders.return_date IS NOT NULL THEN FORMAT(orders.return_date,'dd-MM-yyyy') END AS 'Return date'," +
				"CASE WHEN orders.return_date IS NULL THEN datediff(day,orders.due_date,GETDATE())" +
					  "WHEN orders.return_date IS NOT NULL THEN datediff(day,orders.rent_date,due_date)" +
					  " END AS 'Days' " +
				"from orders " +
				"join movies on movies.id=orders.movie_id " +
				"join clients on clients.id=orders.client_id";

            //Get list of filters and append to query with ANDs inbetween
            var Filters = new ArrayList();

            //Filter by Last Name 
            if (!string.IsNullOrWhiteSpace(FilterOrderClientString.Text))
            { Filters.Add(" clients.last_name LIKE \'%" + FilterOrderClientString.Text + "%\'"); }

            //Filter by Movie 
            if (!string.IsNullOrWhiteSpace(FilterOrderMovieString.Text))
            { Filters.Add(" movies.name LIKE \'%" + FilterOrderMovieString.Text + "%\'"); }

            //Filter by Year 
            if (!string.IsNullOrWhiteSpace(FilterOrderYearString.Text))
            { Filters.Add(" movies.year LIKE \'%" + FilterOrderYearString.Text + "%\'"); }

			
			//dates
			string date;

			//Filter by Rent Date
			date = FilterOrderRentDateString.Text;
            if (!string.IsNullOrWhiteSpace(date))
			{ Filters.Add(" orders.rent_date >= CONVERT(datetime,\'" + date + "\',105)"); }

            //Filter by Due Date
            date = FilterOrderDueDateString.Text;
            if (!string.IsNullOrWhiteSpace(date))
			{ Filters.Add(" orders.due_date <= CONVERT(datetime,\'" + date + "\',105)"); }

            //Filter by Return Date
            date = FilterOrderReturnDateString.Text;
            if (!string.IsNullOrWhiteSpace(date))
			{ Filters.Add(" orders.return_date <= CONVERT(datetime,\'" + date + "\',105)"); }

			//If filtering enabled add filters to query
			if (Filters.Count > 0)
            {
                query += " WHERE ";

                if (Filters.Count > 1)
                    for (int i = 0; i < Filters.Count; i++)
                    {
                        query += Filters[i];
                        if (i < Filters.Count - 1) { query += " AND "; }
                    }
                else { query += Filters[0]; }
            }

            //query
            try
            {
                getquery(orders_dt, query);
            }
            catch (Exception ex)
            {
                Error.Text = ex.Message;
            }
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
		public void AttributesComboboxesRefresh()
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
		
		//Queries
		private void QueryExecuteButton_Click(object sender, RoutedEventArgs e)
		{
			SQLquery();
        }//sql panel submit button
        public void setquery(string query)
		{
			try
			{
				string strConnection = Properties.Settings.Default.WPF_DBConnectionString;
				SqlConnection con = new SqlConnection(strConnection);
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
        }   //nonquery with error feedback
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
        }//for sql panel; select getquery or setquery based on select existence
        public void getquery(DataTable dt, string query)
		{
			try
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
			catch (Exception ex) { Error.Text=ex.Message; }
        }//refresh datatable to refresh grid items
        public void getquery(DataGrid grid, string query)
		{
			try 
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
			catch (Exception ex) { Error.Text=ex.Message; }
        }//query to grid used in sql panel
        public string getquery(string query)
		{
			try
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
				return dtRecord.Rows[0][0].ToString();
			}
			catch (Exception ex) { Error.Text=ex.Message; return null; }
        }//old query for single row to string
         
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
        }//Combobox items clear and fill (refresh)
		//ID getters
        public int getmovie_id() {
			//get selected item id from selected row
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
			return int.Parse(rowview.Row["ID"].ToString());
		}
		public int getclient_id()
		{
			//get selected item id from selected row
			DataRowView rowview = ClientsCatalog.SelectedItem as DataRowView;
			return int.Parse(rowview.Row["ID"].ToString());
		}
		public int getorder_id()
		{
			//get selected item id from selected row
			DataRowView rowview = OrdersCatalog.SelectedItem as DataRowView;
			return int.Parse(rowview.Row["ID"].ToString());
		}
		
		//TrailerTab
		private void TrailerInit(object sender, RoutedEventArgs e)
		{
			//seek slider init
			//SeekSlider.Maximum = Trailer.NaturalDuration.TimeSpan.TotalMilliseconds; //null pointer exception
			//Trailer.LoadedBehavior = MediaState.Manual;

			TotalTime = Trailer.NaturalDuration.TimeSpan;

			// Create a timer that will update the counters and the time slider
			DispatcherTimer timerVideoTime = new DispatcherTimer();
			timerVideoTime.Interval = TimeSpan.FromSeconds(1);
			timerVideoTime.Tick += new EventHandler(timer_Tick);
			timerVideoTime.Start();
			Trailer.Play();
			Trailer.Pause();
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
		
		private void LoadTrailer(object sender, RoutedEventArgs e)
		{
				//set movie info in poster panel
				DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
				MovieTrailerID.Text = rowview.Row["ID"].ToString();
				MovieTrailerTitle.Text = rowview.Row["Title"].ToString();
				MovieTrailerYear.Text = rowview.Row["Year"].ToString();

				//get path to file
				string trailer_path = getquery("select trailer_path from movies where id=" + MovieTrailerID.Text);
				//try to set if path not empty
				if (trailer_path != "")
				{
					Trailer.Source = new Uri(trailer_path, UriKind.Absolute);
					//Console.WriteLine(Trailer.Source);
					MessageBox.Show("Trailer set in trailer tab. If empty; file not found. You can remove link in poster tab");
				}
				//poster not set
				else if (trailer_path == "") MessageBox.Show("Trailer for this movie is not set. Set it in trailer tab");
        }//Load existing trailer from database using trailer_path on right click menu
        private void SetTrailer(object sender, MouseButtonEventArgs e)
		{
			if (MovieTrailerID.Text != "ID")
			{
				OpenFileDialog op = new OpenFileDialog();
				op.Title = "Select trailer";
				if (op.ShowDialog() == true)
				{
					Trailer.Source = new Uri(op.FileName, UriKind.Absolute);
					//Console.WriteLine(Trailer.Source);

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
							MessageBox.Show("Trailer set");
							MoviesGridRefresh(null, null);
						}
						else if (result == "0") MessageBox.Show("Failed to set trailer");
					}
				}
			}
			else MessageBox.Show("Load trailer from movies catalog");
        }//Set trailer file path and update database
        public void RemoveTrailer(object sender, MouseButtonEventArgs e)
		{
			if (MovieTrailerID.Text != "ID")
			{
				Trailer.Source = null;
				//update database
				setquery("update movies set trailer_path=null where id=" + MovieTrailerID.Text);
				MoviesGridRefresh(null, null);
			}
			else if (MoviePosterID.Text == "ID") MessageBox.Show("Load trailer from movies catalog first");
		}
		private void RemoveTrailer(object sender, RoutedEventArgs e)
		{
			RemoveTrailer(null,null);
        }//context menu remove trailer

        //PosterTab
        private void RemovePoster(object sender, RoutedEventArgs e)
		{
			if (MoviePosterID.Text != "ID")
			{
				Poster.Source = null;
				//remove movie poster from database
				setquery("update movies set poster_path=null where id=" + MoviePosterID.Text);
				MoviesGridRefresh(null, null);
			}
			else if (MoviePosterID.Text == "ID") MessageBox.Show("Load poster from movies catalog first");
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
							MessageBox.Show("Poster set");
							MoviesGridRefresh(null,null);
						}
						else if (result == "0") MessageBox.Show("Failed to set poster");
					}
					MoviesGridRefresh(null,null);
				}
			}
			else if (MoviePosterID.Text == "ID") MessageBox.Show("Load poster from movies catalog");
		}
		private void LoadPoster(object sender, RoutedEventArgs e)
		{
				Poster.Source = null;

				//set movie info in poster panel
				DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
				MoviePosterID.Text = rowview.Row["ID"].ToString();
				MoviePosterTitle.Text = rowview.Row["Title"].ToString();
				MoviePosterYear.Text = rowview.Row["Year"].ToString();

				//get path to file if value present
				string poster_path = getquery("select poster_path from movies where id=" + getmovie_id());

			if (poster_path != "")
			{
				Poster.Source = new BitmapImage(new Uri(poster_path, UriKind.Absolute));
				MessageBox.Show("poster set in poster tab. If empty; file not found. You can remove link in poster tab");
			}
			else if (poster_path == "") MessageBox.Show("poster not set; you can set it in poster tab.");
        }//load poster if exists
		
		//FormTab
        private void SubmitOrder(object sender, MouseButtonEventArgs e)
		{
			//check if client exists (in case of value insertion instead of selection)
			bool exists = false;
			foreach (string item in OrderFormClientLNFNID.Items)
			{
				exists = item == OrderFormClientLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) OrderFormClientLNFNID.Text = "null 1";

			//Proceed on client and movie set
			if (OrderFormClientLNFNID.Text != "null 1" && OrderFormMovieID.Text != "")
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
				if (edit_mode == false && copies_left > 0)
				{
					//date creation preparation with default values
					int rentdd = int.Parse(getquery("select day(GETDATE())"));
					int rentmm = int.Parse(getquery("select month(GETDATE())"));
					int rentyy = int.Parse(getquery("select year(GETDATE())"));

					int duedd = int.Parse(getquery("select day(GETDATE()+3)"));
					int duemm = int.Parse(getquery("select month(GETDATE()+3)"));
					int dueyy = int.Parse(getquery("select year(GETDATE()+3)"));
					
					//var for all date error strings
					string msg = "";

					//dates validation. Defaults if not valid. Else set correct values
					DateTime test;
					if (!DateTime.TryParse(OrderFormRentDate.Text, out test)) msg = "Invalid rent date. Today will be set\n";
					else
					{
						rentdd = test.Day;
						rentmm = test.Month;
						rentyy = test.Year;
					}
					if (!DateTime.TryParse(OrderFormDueDate.Text, out test)) msg += "Invalid due date. Today +3 days will be set\n";
					else
					{
						//set due date if not lower than rent date
						DateTime testRentDate = new DateTime(rentyy, rentmm, rentdd);
						DateTime testDueDate = new DateTime(test.Year, test.Month, test.Day);
						
						int correct = testDueDate.CompareTo(testRentDate);
						if (correct >= 0)
						{
							duedd = test.Day;
							duemm = test.Month;
							dueyy = test.Year;
						}
						else msg += "Due date lower than rent date. Today +3 days will be set\n";
					}
					DateTime RentDate = new DateTime(rentyy, rentmm, rentdd);
					DateTime DueDate = new DateTime(dueyy, duemm, duedd);

					//show all date errors in 1 message
					if (msg != "") MessageBox.Show(msg);
					//dates might exceed valid values
					try 
					{
						using (var connection = new SqlConnection(Properties.Settings.Default.WPF_DBConnectionString))
						{
							connection.Open();
							var sql = @"Insert into Orders (client_id,movie_id,rent_date,due_date) " +
								"values(@client_id,@movie_id," +
								"@rent_date,@due_date)";
							using (var cmd = new SqlCommand(sql, connection))
							{
								cmd.Parameters.AddWithValue("@client_id", movie_id);
								cmd.Parameters.AddWithValue("@movie_id", OrderFormMovieID.Text);
								cmd.Parameters.AddWithValue("@rent_date", RentDate);
								cmd.Parameters.AddWithValue("@due_date", DueDate);
								MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
								OrdersGridRefresh(null, null);
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
								MoviesGridRefresh(null, null);
							}
						}
					}
					catch (Exception ex ){ Error.Text=ex.Message; }
				}
				//add mode no copies left
				else if (edit_mode == false && copies_left <= 0) MessageBox.Show("No copies left to rent");
				//edit mode
				else if (edit_mode == true && OrderFormID.Text != "")
				{
					//date creation preparation with default values
					int rentdd = int.Parse(getquery("select day(GETDATE())"));
					int rentmm = int.Parse(getquery("select month(GETDATE())"));
					int rentyy = int.Parse(getquery("select year(GETDATE())"));

					int duedd = int.Parse(getquery("select day(GETDATE()+3)"));
					int duemm = int.Parse(getquery("select month(GETDATE()+3)"));
					int dueyy = int.Parse(getquery("select year(GETDATE()+3)"));

					int returndd = int.Parse(getquery("select day(GETDATE())"));
					int returnmm = int.Parse(getquery("select month(GETDATE())"));
					int returnyy = int.Parse(getquery("select year(GETDATE())"));

					//var for all date error strings
					string msg="";

					//dates validation. Defaults if not valid. Else set correct values
					DateTime test;
					if (!DateTime.TryParse(OrderFormRentDate.Text, out test)) msg = "Invalid rent date. Today will be set\n";
					else
					{
						rentdd = test.Day;
						rentmm = test.Month;
						rentyy = test.Year;
					}
					if (!DateTime.TryParse(OrderFormDueDate.Text, out test)) msg += "Invalid due date. Today +3 days will be set\n";
					else
					{
						//set due date if not lower than rent date
						DateTime testRentDate = new DateTime(rentyy, rentmm, rentdd);
						DateTime testDueDate = new DateTime(test.Year, test.Month, test.Day);

						int later = testDueDate.CompareTo(testRentDate);
						if (later >= 0)
						{
							duedd = test.Day;
							duemm = test.Month;
							dueyy = test.Year;
						}
						else msg += "Due date lower than rent date. Today +3 days will be set\n";
					}
					if (!DateTime.TryParse(OrderFormReturnDate.Text, out test)) msg += "Invalid return date. Today will be set";
					else
					{
						//set due date if not lower than rent date
						DateTime testDueDate = new DateTime(test.Year, test.Month, test.Day);
						DateTime testReturnDate = new DateTime(returnyy, returnmm, returndd);

						int later = testDueDate.CompareTo(testReturnDate);
						if (later >= 0)
						{
							returndd = test.Day;
							returnmm = test.Month;
							returnyy = test.Year;
						}
						else msg += "Return date lower than due date. Today will be set\n";
					}
					DateTime RentDate = new DateTime(rentyy, rentmm, rentdd);
					DateTime DueDate = new DateTime(dueyy, duemm, duedd);
					DateTime ReturnDate = new DateTime(returnyy, returnmm, returndd);

					//show all date errors in 1 message
					if (msg != "") MessageBox.Show(msg);
					//dates might exceed valid values
					try
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
								cmd.Parameters.AddWithValue("@client_id", OrderFormClientLNFNID.Text.Split(' ').Last());
								cmd.Parameters.AddWithValue("@movie_id", OrderFormMovieID.Text);
								cmd.Parameters.AddWithValue("@rent_date", RentDate);
								cmd.Parameters.AddWithValue("@due_date", DueDate);
								cmd.Parameters.AddWithValue("@return_date", ReturnDate);
								MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
								OrdersGridRefresh(null, null);
							}
						}
					}
					catch ( Exception ex ){ Error.Text=ex.Message; }
				}else if (edit_mode == true && OrderFormID.Text == "") MessageBox.Show("Order id not set. Use edit context menu option in orders catalog");
			}else MessageBox.Show("Select movie to rent in catalog and select client");
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
				if (edit_mode == false && ClientFormFirstName.Text != "" && ClientFormLastName.Text != "")
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
								ClientsGridRefresh(null,null);
								ClientsComboboxRefresh();
								ClientClearForm();
							}
						}
				} else if (edit_mode == false && ClientFormFirstName.Text == "" && ClientFormLastName.Text == "") MessageBox.Show("First name and last name must be filled correctly");
			//edit mode
			else if (edit_mode == true && ClientFormID.Text != "")
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
							ClientsGridRefresh(null,null);
							ClientsComboboxRefresh();
						}
					}
				}
				else MessageBox.Show("To set client use edit context menu option in clients catalog");
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
			if (exists == false) MovieFormCountryNameID.Text = "- 1";

			//lang
			exists = false;
			foreach (string item in MovieFormLangNameID.Items)
			{
				exists = item == MovieFormLangNameID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormLangNameID.Text = "- 1";

			//genre
			exists = false;
			foreach (string item in MovieFormGenreNameID.Items)
			{
				exists = item == MovieFormGenreNameID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormGenreNameID.Text = "- 1";

			//director
			exists = false;
			foreach (string item in MovieFormDirectorLNFNID.Items)
			{
				exists = item == MovieFormDirectorLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormDirectorLNFNID.Text = "- 1";

			//actor
			exists = false;
			foreach (string item in MovieFormActorLNFNID.Items)
			{
				exists = item == MovieFormActorLNFNID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormActorLNFNID.Text = "- 1";

			//format
			exists = false;
			foreach (string item in MovieFormFormatNameID.Items)
			{
				exists = item == MovieFormFormatNameID.Text;
				if (exists) break;
			}
			if (exists == false) MovieFormFormatNameID.Text = "- 1";

			//get country id from combobox
			string country_id = "1";
			if (MovieFormCountryNameID.Text != "- 1")country_id = MovieFormCountryNameID.Text.Split(' ').Last();
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
			string plot = "";
			if (MovieFormPlot.Text != "")				plot = MovieFormPlot.Text;
			//get lang_id from combobox
			string lang_id = "1";
			if (MovieFormLangNameID.Text != "- 1")		lang_id = MovieFormLangNameID.Text.Split(' ').Last();
			//get actor_id from combobox
			string actor_id = "1";
			if (MovieFormActorLNFNID.Text != "- 1")	actor_id = MovieFormActorLNFNID.Text.Split(' ').Last();
			//get director_id from combobox
			string director_id = "1";
			if (MovieFormDirectorLNFNID.Text != "- 1") director_id = MovieFormDirectorLNFNID.Text.Split(' ').Last();
			//get format_id from combobox
			string format_id = "1";
			if (MovieFormFormatNameID.Text != "- 1")	format_id = MovieFormFormatNameID.Text.Split(' ').Last();
			//get genre_id from combobox
			string genre_id = "1";
			if (MovieFormFormatNameID.Text != "- 1")	genre_id = MovieFormGenreNameID.Text.Split(' ').Last();
			
			//poster_path and trailer_path must be set in their respective tabs
			
			//add mode
			if (edit_mode == false && MovieFormTitle.Text != "")
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
							MoviesGridRefresh(null,null);
							MovieClearForm();
						}
					}
			}
			//add mode no title set
			else if (edit_mode == false && MovieFormTitle.Text == "") MessageBox.Show("Fill movie title");
			//edit mode
			else if (edit_mode = true && MovieFormID.Text != "")
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
							MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
							MoviesGridRefresh(null,null);
						}
					}
			}else MessageBox.Show("Movie id not set. Use edit context menu option from movies catalog");
		}
		
		//Clearing Forms
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
			MovieFormPlot.Text = null;

			//comboboxes
			MovieFormGenreNameID.Text = "- 1";
			MovieFormFormatNameID.Text = "- 1";
			MovieFormDirectorLNFNID.Text = "- 1";
			MovieFormActorLNFNID.Text = "- 1";
			MovieFormCountryNameID.Text = "- 1";
			MovieFormLangNameID.Text = "- 1";
        }
        public void ClientClearForm()
		{
			ClientFormID.Text = null;
			ClientFormLastName.Text = null;
			ClientFormFirstName.Text = null;
			ClientFormEmail.Text = null;
			ClientFormPhone.Text = null;
		}
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
        
		//MoviesTab right click menu
        private void MovieItem_plot(object sender, RoutedEventArgs e)//Show Plot
        {
            DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
            string plot = rowview.Row["Plot"].ToString();
            MessageBox.Show(plot);
        }
        private void MovieItem_rent(object sender, RoutedEventArgs e)//Fill Rent Form 
		{
			Tabs.SelectedIndex = 5;
			//add mode
			SelectAddMode(null,null);
			//autofill form
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;

			OrderFormID.Text = null;
			OrderFormMovieID.Text = rowview.Row["ID"].ToString();

			//combobox
			OrderFormClientLNFNID.Text = null;

			//set default dates
			OrderFormRentDate.Text = getquery("select GETDATE()");
			OrderFormDueDate.Text = getquery("select GETDATE()+3");
			OrderFormReturnDate.Text = null;

			MessageBox.Show("Movie data set. Submit order in admin panel.");
		}
		private void MovieItem_edit(object sender, RoutedEventArgs e)
		{
            Tabs.SelectedIndex = 1;
            MovieClearForm();
			//edit mode
			SelectEditMode(null, null);
			
			//autofill form
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;

			MovieFormID.Text = rowview.Row["ID"].ToString();
			MovieFormTitle.Text = rowview.Row["Title"].ToString();
			MovieFormYear.Text = rowview.Row["Year"].ToString();
			MovieFormDuration.Text = rowview.Row["Duration"].ToString();
			MovieFormAge.Text = rowview.Row["Age"].ToString();
			MovieFormPrice.Text = rowview.Row["Price"].ToString();
			MovieFormCopiesLeft.Text = rowview.Row["Left copies"].ToString();
			MovieFormCopiesTotal.Text = rowview.Row["All copies"].ToString();
			MovieFormPlot.Text = rowview.Row["Plot"].ToString();

			//comboboxes
			MovieFormGenreNameID.Text = rowview.Row["Genre"].ToString();
			MovieFormFormatNameID.Text = rowview.Row["Format"].ToString();
			MovieFormDirectorLNFNID.Text = rowview.Row["Director"].ToString();
			MovieFormActorLNFNID.Text = rowview.Row["Lead actor"].ToString();
			MovieFormCountryNameID.Text = rowview.Row["Country"].ToString();
			MovieFormLangNameID.Text = rowview.Row["Language"].ToString();

			MessageBox.Show("Movie data set. Edit and submit changes in admin panel.");
		}
		private void MovieItem_delete(object sender, RoutedEventArgs e)
		{
			//clear poster and trailer
				MovieTrailerID.Text = "ID";
				MovieTrailerTitle.Text = "Title";
				MovieTrailerYear.Text = "Year";
				Trailer.Source = null;
			
				MoviePosterID.Text = "ID";
				MoviePosterTitle.Text = "Title";
				MoviePosterYear.Text = "Year";
				Poster.Source = null;
			
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
					catch (Exception /*ex*/)
					{
					MessageBox.Show("Couldn't remove row referenced from other row");
					}
				}
				MoviesGridRefresh(null,null);
		}
		private void MovieItem_trailer(object sender, RoutedEventArgs e)
		{
			TrailerInit(null, null);
		}
		
		//ClientsTab right click menu
		//Fills existing client data into form in admin panel (edit client option)
		private void ClientItem_edit(object sender, RoutedEventArgs e)
		{
            Tabs.SelectedIndex = 5;
            ClientClearForm();
			//edit mode
			SelectEditMode(null, null);
			
			//fill form
			ClientFormID.Text = getclient_id().ToString();
			DataRowView rowview = ClientsCatalog.SelectedItem as DataRowView;
			ClientFormLastName.Text = rowview.Row["Last name"].ToString();
			ClientFormFirstName.Text = rowview.Row["First name"].ToString();
			ClientFormEmail.Text = rowview.Row["Email"].ToString();
			ClientFormPhone.Text = rowview.Row["Phone"].ToString();

			//MessageBox.Show("Client data set. Submit in admin panel.");
            MessageBar.Text = "Client data set. Submit in admin panel.";
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
				catch (Exception /*ex*/)
				{
					//MessageBox.Show("Couldn't remove row referenced from other row.");
                    MessageBar.Text = "Couldn't remove row referenced from other row.";
                }
			}
			ClientsGridRefresh(null,null);
		}
		
		//OrdersTab
		//Orders right click menu
		private void OrderItem_return(object sender, RoutedEventArgs e)
		{
            Tabs.SelectedIndex = 5;
            //get selected row
            DataRowView rowview = OrdersCatalog.SelectedItem as DataRowView;
			//check null on return date
			string is_returned = rowview.Row["Return date"].ToString();

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
				MoviesGridRefresh(null, null);
				OrdersGridRefresh(null, null);
			}
			else 
			{
				//MessageBox.Show("Movie already returned");
				MessageBar.Text = "Movie already returned";
            }
		}
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
			string movie_id = rowview.Row["Movie"].ToString().Split().Last();
			OrderFormMovieID.Text = movie_id;

			//get client name id
			string client = rowview.Row["Client"].ToString();
			OrderFormClientLNFNID.Text = client;

			//get rent date
			string rent_date = rowview.Row["Rent date"].ToString();
			OrderFormRentDate.Text = rent_date;
			
			//get due date
			string due_date = rowview.Row["Due date"].ToString();
			OrderFormDueDate.Text = due_date;
			
			//get return date
			string return_date = rowview.Row["Return date"].ToString();
			OrderFormReturnDate.Text = return_date;
			
			//MessageBar.Text = "Order data set. Edit order in admin panel.";
            MessageBar.Text = "Order data set. Edit order in admin panel.";
        }//Fills existing order data into form in admin panel (edit order option)
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
			OrdersGridRefresh(null,null);
		}
		
		//Form Submit Mode Selector
		bool edit_mode = false;//Selected action mode on submit form. Add on False, overwrite on True
        private void SelectAddMode(object sender, MouseButtonEventArgs e)
		{
			edit_mode = false;
			ModeSelected.Text = "You are now in ADD mode";
			ModeSelected.Foreground = Brushes.LightGreen;
            ModeSelected.Background = Brushes.Black;
            //set radial background
            RadialGradientBrush radialGradient = new RadialGradientBrush();
            radialGradient.GradientStops.Add(new GradientStop(Colors.Black, 0.8));
            radialGradient.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));
            radialGradient.Freeze();

            ModeSelected.Background = radialGradient;
            SubmitPanel.Background = radialGradient;

            //disable order return date
            OrderFormReturnDate.IsEnabled = false;
		}
		private void SelectEditMode(object sender, MouseButtonEventArgs e)
		{
			edit_mode = true;
			ModeSelected.Text = "You are now in EDIT mode";
			ModeSelected.Foreground = Brushes.Yellow;
            ModeSelected.Background = Brushes.Black;
            //set radial background
            RadialGradientBrush radialGradient = new RadialGradientBrush();
            radialGradient.GradientStops.Add(new GradientStop(Colors.Black, 0.8));
            radialGradient.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));
            radialGradient.Freeze();

            ModeSelected.Background = radialGradient;
            SubmitPanel.Background = radialGradient;

            //enable order return date
            OrderFormReturnDate.IsEnabled = true;
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
		//AttributePanel
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
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
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
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
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
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
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
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
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
					MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
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
			if (exists == false) UpdateActorLNFNID.Text = "- 1";

			if (UpdateActorLNFNID.Text != "- 1")
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
				//refresh actors combobox
				ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
				ComboboxRefresh(UpdateActorLNFNID, "select last_name,first_name,id from actors");
				ComboboxRefresh(DeleteActorLNFNID, "select actors.last_name,actors.first_name,actors.id from actors left outer join movies on actors.id=movies.actor_id where movies.actor_id is null");
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
			if (exists == false) UpdateDirectorLNFNID.Text = "- 1";

			if (UpdateDirectorLNFNID.Text != "- 1")
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
				//refresh directors combobox
				ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
				ComboboxRefresh(UpdateDirectorLNFNID, "select last_name,first_name,id from directors");
				ComboboxRefresh(DeleteDirectorLNFNID, "select directors.last_name,directors.first_name,directors.id from directors left outer join movies on directors.id=movies.director_id where movies.director_id is null");

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
			if (exists == false) UpdateCountryNameID.Text = "- 1";

			if (UpdateCountryNameID.Text != "- 1")
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
				//refresh countries combobox
				ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
				ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");
				ComboboxRefresh(DeleteCountryNameID, "select countries.name,countries.id from countries left outer join movies on countries.id=movies.country_id where movies.country_id is null");
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
			if (exists == false) UpdateLangNameID.Text = "- 1";

			if (UpdateLangNameID.Text != "- 1")
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
				ComboboxRefresh(DeleteLangNameID, "select langs.name,langs.id from langs left outer join movies on langs.id=movies.lang_id where movies.lang_id is null");
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
			if (exists == false) UpdateFormatNameID.Text = "- 1";

			if (UpdateFormatNameID.Text != "- 1")
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
				ComboboxRefresh(MovieFormFormatNameID, "select name,id from formats");
				ComboboxRefresh(UpdateFormatNameID, "select name,id from formats");
				ComboboxRefresh(DeleteFormatNameID, "select formats.name,formats.id from formats left outer join movies on formats.id=movies.format_id where movies.format_id is null");
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
			if (exists == false) UpdateGenreNameID.Text = "- 1";

			if (UpdateGenreNameID.Text != "- 1")
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
				ComboboxRefresh(DeleteGenreNameID, "select genres.name,genres.id from genres left outer join movies on genres.id=movies.genre_id where movies.genre_id is null");
			}
			else MessageBox.Show("No item selected");
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
			if (exists == false) DeleteActorLNFNID.Text = "- 1";
			
			if (DeleteActorLNFNID.Text != "- 1")
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
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						Error.Text=ex.Message;
					}
				}
				//refresh actors combobox
				ComboboxRefresh(MovieFormActorLNFNID, "select last_name,first_name,id from actors");
				ComboboxRefresh(UpdateActorLNFNID, "select last_name,first_name,id from actors");
				ComboboxRefresh(DeleteActorLNFNID, "select actors.last_name,actors.first_name,actors.id from actors left outer join movies on actors.id=movies.actor_id where movies.actor_id is null");
			}
			else MessageBox.Show("No item selected");
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
			if (exists == false) DeleteDirectorLNFNID.Text = "- 1";

			if (DeleteDirectorLNFNID.Text != "- 1")
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
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						Error.Text=ex.Message;
					}
				}
				//setquery("delete from directors where id=" + director_id + ")");
				//refresh directors combobox
				ComboboxRefresh(MovieFormDirectorLNFNID, "select last_name,first_name,id from directors");
				ComboboxRefresh(UpdateDirectorLNFNID, "select last_name,first_name,id from directors");
				ComboboxRefresh(DeleteDirectorLNFNID, "select directors.last_name,directors.first_name,directors.id from directors left outer join movies on directors.id=movies.director_id where movies.director_id is null");
			}
			else MessageBox.Show("No item selected");
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
			if (exists == false) DeleteCountryNameID.Text = "- 1";

			if (DeleteCountryNameID.Text != "- 1")
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
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						Error.Text=ex.Message;
					}
				}
				//refresh countries combobox
				ComboboxRefresh(MovieFormCountryNameID, "select name,id from countries");
				ComboboxRefresh(UpdateCountryNameID, "select name,id from countries");
				ComboboxRefresh(DeleteCountryNameID, "select countries.name,countries.id from countries left outer join movies on countries.id=movies.country_id where movies.country_id is null");
			}
			else MessageBox.Show("No item selected");
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
			if (exists == false) DeleteLangNameID.Text = "- 1";

			if (DeleteLangNameID.Text != "- 1")
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
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						Error.Text=ex.Message;
					}
				}
				//refresh langs combobox
				ComboboxRefresh(MovieFormLangNameID, "select name,id from langs");
				ComboboxRefresh(UpdateLangNameID, "select name,id from langs");
				ComboboxRefresh(DeleteLangNameID, "select langs.name,langs.id from langs left outer join movies on langs.id=movies.lang_id where movies.lang_id is null");
			}
			else MessageBox.Show("No item selected");
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
			if (exists == false) DeleteFormatNameID.Text = "- 1";

			if (DeleteFormatNameID.Text != "- 1")
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
						MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
					}
					catch (Exception ex)
					{
						Error.Text=ex.Message;
					}
				}
				//setquery("delete from formats where id=" + format_id + ")");
				//refresh formats combobox
				ComboboxRefresh(MovieFormFormatNameID, "select name,id from formats");
				ComboboxRefresh(UpdateFormatNameID, "select name,id from formats");
				ComboboxRefresh(DeleteFormatNameID, "select formats.name,formats.id from formats left outer join movies on formats.id=movies.format_id where movies.format_id is null");
			}
			else MessageBox.Show("No item selected");
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
			if (exists == false) DeleteGenreNameID.Text = "- 1";

			if (DeleteGenreNameID.Text != "- 1")
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
							MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery().ToString());
						}
						catch (Exception ex)
						{
							Error.Text=ex.Message;
						}
					}
					//refresh genres combobox
					ComboboxRefresh(MovieFormGenreNameID, "select name,id from genres");
					ComboboxRefresh(UpdateGenreNameID, "select name,id from genres");
					ComboboxRefresh(DeleteGenreNameID, "select genres.name,genres.id from genres left outer join movies on genres.id=movies.genre_id where movies.genre_id is null");
			}
			else MessageBox.Show("No item selected");
		}
		//row counters
		private void MoviesCatalog_LoadingRow(object sender, DataGridRowEventArgs e)
		{
			MoviesCount.Text = "Rows: "+ MoviesCatalog.Items.Count.ToString();
		}
		private void ClientsCatalog_LoadingRow(object sender, DataGridRowEventArgs e)
		{
			ClientsCount.Text = "Rows: " + ClientsCatalog.Items.Count.ToString();
		}
		private void OrdersCatalog_LoadingRow(object sender, DataGridRowEventArgs e)
		{
			OrdersCount.Text = "Rows: " + OrdersCatalog.Items.Count.ToString();
		}
	}
}