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
		private void FilterTitle_MouseDoubleClick(object sender,MouseButtonEventArgs e)
		{
			FilterTitle.Text = "filter";
		}
		private void FilterYear_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterYear.Text = "filter";
		}
		private void FilterAge_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterAge.Text = "filter";
		}
		private void FilterDuration_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterDuration.Text = "filter";
		}
		private void FilterLanguages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterLanguages.Text = "filter";
		}
		private void FilterDirectors_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterDirectors.Text = "filter";
		}
		private void FilterFormats_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterFormats.Text = "filter";
		}
		private void FilterPrice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterPrice.Text = "filter";
		}
		private void FilterCountries_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterCountries.Text = "filter";
		}
		private void FilterLeadActors_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterLeadActors.Text = "filter";
		}
		private void FilterCopiesLeft_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterCopiesLeft.Text = "filter";
		}
		private void FilterCopiesTotal_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterCopiesTotal.Text = "filter";
		}

		private void Element_MediaEnded(object sender, RoutedEventArgs e)
		{

        }

		private void Element_MediaOpened(object sender, RoutedEventArgs e)
		{

		}

		private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void OnMouseDownPlayMedia(object sender, RoutedEventArgs e)
		{

		}

		private void OnMouseDownPauseMedia(object sender, RoutedEventArgs e)
		{

		}

		private void OnMouseDownStopMedia(object sender, RoutedEventArgs e)
		{

		}

		private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void Duration_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterPremise_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterLastName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterFirstName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterEmail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterPhone_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterLName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterFName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterDueDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterReturnDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterRentDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void RemovePoster(object sender, RoutedEventArgs e)
		{

        }

		private void ChangePoster(object sender, RoutedEventArgs e)
		{

		}

		private void AddPoster(object sender, RoutedEventArgs e)
		{

		}

		private void AdjustVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void PlayTrailer(object sender, RoutedEventArgs e)
		{

		}

		private void ResetTrailer(object sender, RoutedEventArgs e)
		{

		}

		private void SeekTrailer(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void Trailer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{

		}

		private void Trailer_MediaFailed_1(object sender, ExceptionRoutedEventArgs e)
		{

		}

		private void TrailerFailed(object sender, ExceptionRoutedEventArgs e)
		{

		}

		private void TrailerFile(object sender, RoutedEventArgs e)
		{

		}

		private void ClickEditMode(object sender, MouseButtonEventArgs e)
		{

        }

		private void ClickAddMode(object sender, MouseButtonEventArgs e)
		{

		}
	}
}