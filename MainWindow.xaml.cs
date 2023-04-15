using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Entity.Core.Objects;
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
using System.Data.Entity.ModelConfiguration.Configuration;
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
			MoviesGridRefresh(sender, e);
			ClientsGridRefresh(sender, e);
			OrdersGridRefresh(sender, e);
		}
		//sql panel button
		private void QueryExecuteButton_Click(object sender, RoutedEventArgs e)
		{
			SQLquery();
		}
		//query with feedback info
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
			grid.ItemsSource = dtRecord.DefaultView;
		}
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
			return dtRecord.ToString();
		}
		private void MoviesGridRefresh(object sender, RoutedEventArgs e)
		{
			getquery(MoviesCatalog,
				"Select movies.id, movies.name as title, movies.year, movies.duration, movies.age, movies.price," +
				"movies.plot," +
				"formats.name as format," +
				"CONCAT(directors.last_name,directors.first_name) as director," +
				"CONCAT(actors.last_name,actors.first_name) as lead_actor," +
				"countries.name as country, langs.name as language," +
				"movies.copies_left, movies.copies_total from movies " +
				"join actors on actors.id = movies.actor_id " +
				"join countries on countries.id = movies.country_id " +
				"join langs on langs.id = movies.lang_id " +
				"join directors on directors.id = movies.director_id " +
				"join formats on formats.id = movies.format_id");
		}
		private void ClientsGridRefresh(object sender, RoutedEventArgs e)
		{
			getquery(ClientsCatalog, "Select * from clients");
		}
		private void OrdersGridRefresh(object sender, RoutedEventArgs e)
		{
			getquery(OrdersCatalog, "Select * from orders");
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
		private void AdjustVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			//Trailer.Volume = (double)VolumeSlider.Value; //null pointer exception
		}
		private void PlayTrailer(object sender, RoutedEventArgs e)
		{
			Trailer.Play();
			//Trailer.Volume = (double)VolumeSlider.Value; //null pointer exception
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
				MoviesGridRefresh(null, null);
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
			MovieFormActor.Text = rowview.Row["lead_actor"].ToString();
			MovieFormDirector.Text = rowview.Row["director"].ToString();
			MovieFormAge.Text = rowview.Row["age"].ToString();
			MovieFormDuration.Text = rowview.Row["duration"].ToString();
			MovieFormCopiesLeft.Text = rowview.Row["copies_left"].ToString();
			MovieFormCopiesTotal.Text = rowview.Row["copies_total"].ToString();
			MovieFormLang.Text = rowview.Row["language"].ToString();
			MovieFormFormat.Text = rowview.Row["format"].ToString();
			MovieFormGenre.Text = rowview.Row["genre"].ToString();
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
			OrderFormMovieTitleID.Text = getquery("select title from movies where id=" + movie_id) + " " + movie_id;
			//set client
			int client_id = int.Parse(rowview.Row["client_id"].ToString());
			OrderFormClientFNLNID.Text = getquery("select first_name from clients where id=" + client_id);
			OrderFormClientFNLNID.Text += " " + getquery("select last_name from clients where id=" + client_id);
			OrderFormClientFNLNID.Text += " " + client_id;

			//get date in format dd/mm/yy (30/12/2022)
			//set rent date
			string rent_date = getquery("select convert(varchar, rent_date, 1) from orders where id=" + OrderFormID.Text);
			int dd = int.Parse(rent_date.Split('/')[0]);
			int mm = int.Parse(rent_date.Split('/')[1]);
			int yy = int.Parse(rent_date.Split('/')[2]);
			OrderFormRentDate.SelectedDate=new DateTime(yy, mm, dd);

			//set due date
			string due_date = getquery("select convert(varchar, due_date, 1) from orders where id=" + OrderFormID.Text);
			dd = int.Parse(rent_date.Split('/')[0]);
			mm = int.Parse(rent_date.Split('/')[1]);
			yy = int.Parse(rent_date.Split('/')[2]);
			OrderFormRentDate.SelectedDate = new DateTime(yy, mm, dd);

			//set return date
			string return_date = getquery("select convert(varchar, return_date, 1) from orders where id=" + OrderFormID.Text);
			dd = int.Parse(rent_date.Split('/')[0]);
			mm = int.Parse(rent_date.Split('/')[1]);
			yy = int.Parse(rent_date.Split('/')[2]);
			OrderFormRentDate.SelectedDate = new DateTime(yy, mm, dd);
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
			MoviesGridRefresh(null, null);
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
			int copies_left = int.Parse(getquery("select copies_left from movies where id="+movie_id));
			setquery("update movies set copies_left=" + copies_left + 1 +" where id=" + movie_id);

			OrdersGridRefresh(sender, e);
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
			OrdersGridRefresh(null, null);
		}
		//Filter movies
		private void FilterMovieTitle(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Return)
			{
			
			}
		}
		private void FilterMoviePrice(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterMovieAge(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterMovieDuration(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterMovieYear(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterMovieCopiesTotal(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterMovieCopiesLeft(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterMovieCountry(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterMovieLang(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterMovieFormat(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterMovieDirector(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterMovieActor(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		//Filter clients
		private void FilterClientLastName(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterClientFirstName(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterClientEmail(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterClientPhone(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		//Filter orders
		private void FilterOrderMovie(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterOrderLastName(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterOrderFirstName(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterOrderRentDate(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterOrderDueDate(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

			}
		}
		private void FilterOrderReturnDate(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{

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
		//Submit Buttons
		private void SubmitOrder(object sender, MouseButtonEventArgs e)
		{
			//refresh grid
		}
		private void SubmitClient(object sender, MouseButtonEventArgs e)
		{
			//refresh grid
		}
		private void SubmitMovie(object sender, MouseButtonEventArgs e)
		{
			//refresh grid
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
		//Add
		private void AddActor(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into actors values(null,"+AddActorFirstName+","+AddActorLastName+")");
		}
		private void AddDirector(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into directors values(null," + AddActorFirstName + "," + AddActorLastName + ")");
		}
		private void AddCountry(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into countries values(null," + AddCountryName + ")");
		}
		private void AddLang(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into langs values(null," + AddLangName + ")");
		}
		private void AddFormat(object sender, MouseButtonEventArgs e)
		{
			setquery("Insert into formats values(null," + AddFormatName + ")");
		}
		//Update
		private void UpdateActor(object sender, MouseButtonEventArgs e)
		{
			//combobox contains last_name first_name id
			string[] tokens = UpdateActorFNLNID.Text.Split(' ');

			setquery("update actors set first_name=" + UpdateActorFirstName
									+ ",last_name=" + UpdateActorLastName
									+ "where id=" + tokens[2] +")");
		}
		private void UpdateDirector(object sender, MouseButtonEventArgs e)
		{
			//combobox contains last_name first_name id
			string[] tokens = UpdateDirectorFNLNID.Text.Split(' ');

			setquery("update directors set first_name=" + UpdateDirectorFirstName
									+ ",last_name=" + UpdateDirectorLastName
									+ "where id=" + tokens[2] + ")");
		}
		private void UpdateCountry(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string[] tokens = UpdateCountryNameID.Text.Split(' ');

			setquery("update countries set name=" + UpdateCountryName
									+ "where id=" + tokens[1] + ")");
		}
		private void UpdateLang(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string[] tokens = UpdateLangNameID.Text.Split(' ');

			setquery("update langs set name=" + UpdateLangName
									+ "where id=" + tokens[1] + ")");
		}
		private void UpdateFormat(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string[] tokens = UpdateFormatNameID.Text.Split(' ');

			setquery("update formats set name=" + UpdateFormatName
									+ "where id=" + tokens[1] + ")");
		}
		//Delete
		private void DeleteActor(object sender, MouseButtonEventArgs e)
		{
			//combobox contains last_name first_name id
			string[] tokens = UpdateActorFNLNID.Text.Split(' ');

			setquery("delete from actors where id=" + tokens[2] + ")");
		}
		private void DeleteDirector(object sender, MouseButtonEventArgs e)
		{
			//combobox contains last_name first_name id
			string[] tokens = UpdateDirectorFNLNID.Text.Split(' ');

			setquery("delete from directors where id=" + tokens[2] + ")");
		}
		private void DeleteCountry(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string[] tokens = UpdateCountryNameID.Text.Split(' ');

			setquery("delete from countries where id=" + tokens[1] + ")");
		}
		private void DeleteLang(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string[] tokens = UpdateLangNameID.Text.Split(' ');

			setquery("delete from langs where id=" + tokens[1] + ")");
		}
		private void DeleteFormat(object sender, MouseButtonEventArgs e)
		{
			//combobox contains name id
			string[] tokens = UpdateFormatNameID.Text.Split(' ');

			setquery("delete from formats where id=" + tokens[1] + ")");
		}
	}
}