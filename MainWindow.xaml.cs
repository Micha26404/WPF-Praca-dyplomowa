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
	
		private void Element_MediaEnded(object sender, RoutedEventArgs e)
		{

        }

		private void Element_MediaOpened(object sender, RoutedEventArgs e)
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

		private void TrailerFailed(object sender, ExceptionRoutedEventArgs e)
		{

		}

		private void TrailerFile(object sender, RoutedEventArgs e)
		{

		}

		private void EditMode(object sender, MouseButtonEventArgs e)
		{

        }

		private void AddMode(object sender, MouseButtonEventArgs e)
		{

		}

		private void SetTrailer(object sender, MouseButtonEventArgs e)
		{

		}

		private void DeleteTrailer(object sender, MouseButtonEventArgs e)
		{

		}

		private void SetPoster(object sender, MouseButtonEventArgs e)
		{

		}

		private void DeletePoster(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterPrice(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterAge(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterDuration(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterYear(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterTitle(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterCopiesTotal(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterCopiesLeft(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterCountries(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterLanguages(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterFormats(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterDirectors(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterLeadActors(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterLastName(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterFirstName(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterEmail(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterPhone(object sender, MouseButtonEventArgs e)
		{

		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void FilterLName(object sender, MouseButtonEventArgs e)
		{

		}
	}
}