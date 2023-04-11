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
			Movies_Click(sender,e);
			Clients_Click(sender, e);
			Orders_Click(sender, e);
		}
		public void query(DataGrid grid, String query) 
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
		private void Movies_Click(object sender, RoutedEventArgs e)
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
		private void Clients_Click(object sender, RoutedEventArgs e)
		{
			query(ClientsCatalog, "Select * from clients");
		}
		private void Orders_Click(object sender, RoutedEventArgs e)
		{
			query(OrdersCatalog, "Select * from orders");
		}
		//Trailer panel
		public double Volume { get; set; }
		private void MediaOpened(object sender, RoutedEventArgs e)
		{
			//seek slider init
			SeekSlider.Maximum = Trailer.NaturalDuration.TimeSpan.TotalMilliseconds;
		}
		private void AdjustVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Trailer.Volume = (double)VolumeSlider.Value;
		}
		private void PlayTrailer(object sender, RoutedEventArgs e)
		{
			Trailer.Play();
			Trailer.Volume = (double)VolumeSlider.Value;
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

			// Overloaded constructor takes the arguments days, hours, minutes, seconds, milliseconds.
			// Create a TimeSpan with miliseconds equal to the slider value.
			TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
			Trailer.Position = ts;
		}
		private void TrailerFile(object sender, RoutedEventArgs e)//Trailer file path
		{

		}
		private void SetTrailer(object sender, MouseButtonEventArgs e)//Set trailer file path
		{

		}
		private void DeleteTrailer(object sender, MouseButtonEventArgs e)
		{

		}
		//Poster panel
		private void RemovePoster(object sender, RoutedEventArgs e)
		{

        }
		private void ChangePoster(object sender, RoutedEventArgs e)
		{

		}
		private void AddPoster(object sender, RoutedEventArgs e)
		{

		}
		private void SetPoster(object sender, MouseButtonEventArgs e)
		{

		}
		private void DeletePoster(object sender, MouseButtonEventArgs e)
		{

		}
		//Admin panel
		private void EditMode(object sender, MouseButtonEventArgs e)
		{

        }
		private void AddMode(object sender, MouseButtonEventArgs e)
		{

		}
		//Filter movies  
		private void FilterPrice_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterAge_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterDuration_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterYear_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterTitle_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterCopiesTotal_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterCopiesLeft_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterCountries_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterLanguages_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterFormats_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterDirectors_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterLeadActors_Click(object sender, MouseButtonEventArgs e)
		{

		}
		//Filter clients
		private void FilterLastName_Click(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterFirstName_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterEmail_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterPhone_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void FilterLName_Click(object sender, MouseButtonEventArgs e)
		{

		}
		private void OrderItem_return(object sender, RoutedEventArgs e)
		{

		}
		private void OrderItem_edit(object sender, RoutedEventArgs e)
		{

		}
		private void ClientItem_add(object sender, RoutedEventArgs e)
		{

		}
		private void ClientItem_edit(object sender, RoutedEventArgs e)
		{

		}
		private void OrderItem_add(object sender, RoutedEventArgs e)
		{

		}
		private void MovieItem_rent(object sender, RoutedEventArgs e)
		{

		}
		private void MovieItem_add(object sender, RoutedEventArgs e)
		{

		}
		private void MovieItem_edit(object sender, RoutedEventArgs e)
		{

		}
		private void MovieItem_delete(object sender, RoutedEventArgs e)
		{

		}
		private void MovieItem_poster(object sender, RoutedEventArgs e)
		{

		}
		private void MovieItem_trailer(object sender, RoutedEventArgs e)
		{

		}
		private void OrderItem_delete(object sender, RoutedEventArgs e)
		{

		}
		
	}
}