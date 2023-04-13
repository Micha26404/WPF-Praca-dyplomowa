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
			MoviesGridRefresh(sender,e);
			ClientsGridRefresh(sender, e);
			OrdersGridRefresh(sender, e);
		}
		public void query(DataGrid grid, string query) 
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
		public string query(string query)
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
			query(MoviesCatalog,
				"Select movies.name as title, movies.year, movies.duration, movies.age as 'age category', movies.price,"+
				"movies.plot as premise,"+
				"formats.name as formats,"+
				"directors.first_name + ' ' + directors.first_name as directors,"+
				"actors.last_name + ' ' + actors.first_name as actors,"+
				"countries.name as countries, langs.name as languages "+
				"from movies "+
				"join actors on actors.id = movies.actor_id "+
				"join countries on countries.id = movies.country_id "+
				"join langs on langs.id = movies.lang_id "+
				"join directors on directors.id = movies.director_id "+
				"join formats on formats.id = movies.format_id");
		}
		private void ClientsGridRefresh(object sender, RoutedEventArgs e)
		{
			query(ClientsCatalog, "Select * from clients");
		}
		private void OrdersGridRefresh(object sender, RoutedEventArgs e)
		{
			query(OrdersCatalog, "Select * from orders");
		}
		public int getmovie_id() {
			//get selected item id from selected row
			DataRowView rowview = MoviesCatalog.SelectedItem as DataRowView;
			return int.Parse(rowview.Row["id"].ToString());
		}
		//Trailer panel
		//action on trailer load
		private void TrailerInit(object sender, RoutedEventArgs e)
		{
			//seek slider init
			//SeekSlider.Maximum = Trailer.NaturalDuration.TimeSpan.TotalMilliseconds;
		}
		private void AdjustVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			//Trailer.Volume = (double)VolumeSlider.Value;
		}
		private void PlayTrailer(object sender, RoutedEventArgs e)
		{
			Trailer.Play();
			//Trailer.Volume = (double)VolumeSlider.Value;
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
			string query_result = query("select trailer_path from movies where id=" + id);
			if (query_result != null) 
			{
				string file= query_result;
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
				query("update movies set poster=" + Trailer.Source + " where id=" + getmovie_id());
			}
		}
		private void DeleteTrailer(object sender, MouseButtonEventArgs e)
		{
			Trailer.Source = null;
			//update database
			query("update movies set trailer_path=null where id="+getmovie_id());
		}
		//Poster panel buttons
		private void RemovePoster(object sender, RoutedEventArgs e)
		{
			Poster.Source = null;
			//remove movie poster from database
			query("update movies set poster=null where id=" + getmovie_id());
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
				Poster.Source = new BitmapImage(new Uri(op.FileName));
				//add poster to movie in database
				query("update movies set poster=" + Poster.Source + " where id=" + getmovie_id());
			}
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
		//Movies right click menu
		private void MovieItem_rent(object sender, RoutedEventArgs e)
		{

		}
		private void MovieItem_edit(object sender, RoutedEventArgs e)
		{
			//set item id in admin panel
		}
		private void MovieItem_delete(object sender, RoutedEventArgs e)
		{
			//refresh grid
		}
		private void MovieItem_poster(object sender, RoutedEventArgs e)
		{
			//load poster
		}
		private void MovieItem_trailer(object sender, RoutedEventArgs e)
		{
			TrailerInit(null, null);
		}
		//Clients right click menu
		private void ClientItem_edit(object sender, RoutedEventArgs e)
		{
			//set item id in admin panel
		}
		//Orders right click menu
		private void OrderItem_return(object sender, RoutedEventArgs e)
		{

		}
		private void OrderItem_edit(object sender, RoutedEventArgs e)
		{
			//set item id in admin panel
		}
		private void OrderItem_delete(object sender, RoutedEventArgs e)
		{
			//refresh grid
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
	}
}