using Orlen.Interfaces;
using Orlen.Model;
using Orlen.Services;
using Orlen.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orlen.ViewModel
{
    public class TimetablePageViewModel : BaseViewModel
    {
        private List<Line> StableLineList;
        List<Line> _timetableList;
        public List<Line> TimetableList
        {
            get
            {
                return _timetableList;
            }
            set
            {
                _timetableList = value;
                OnPropertyChanged();
            }
        }
        string _lineSearchText;
        public string LineSearchText
        {
            get { return _lineSearchText; }
            set
            {
                _lineSearchText = value;
                OnPropertyChanged();
                SearchLine();
            }
        }
        Line _selectedLine;
        public Line SelectedLine
        {
            get { return _selectedLine; }
            set
            {
                _selectedLine = value;
                OnPropertyChanged();
                ShowLinePage();
            }
        }
        private void SearchLine()
        {
            if (string.IsNullOrEmpty(LineSearchText))
                TimetableList = StableLineList;

            TimetableList = TimetableList.FindAll(x => x.LineTo.IndexOf(LineSearchText, StringComparison.OrdinalIgnoreCase) >= 0
                                                    || x.LineNumber.StartsWith(LineSearchText, StringComparison.InvariantCultureIgnoreCase)
                                                    || x.LineTo.IndexOf(LineSearchText, StringComparison.OrdinalIgnoreCase) >= 0
                                                    );
        }

        public TimetablePageViewModel(INavService navService) : base(navService)
        {
            DbService = new DatabaseRepository();
        }
        private async void ShowLinePage()
        {
            if (SelectedLine != null)
            {
                await NavService.NavigateToViewModelParm<LinePageViewModel, Line>(SelectedLine, true);
            }
        }

        public override async Task InitAsync()
        {
            await Task.Factory.StartNew(() =>
            {

                var data = DbService.GetAllBusStops();

                StableLineList = new List<Line>
                {
                    new Line
                    {
                        Id = 1,
                        LineNumber = "10",
                        LineFrom = "Biała",
                        LineTo = "Nowe Trzepowo",
                        lineItems = data.GetRange(0,5)
                    },
                    new Line
                    {
                        Id = 2,
                        LineNumber = "11",
                        LineFrom = "KolTrans",
                        LineTo = "Zglenickiego",
                        lineItems = data.GetRange(4,5),

            },
                    new Line
                    {
                        Id = 3,
                        LineNumber = "12",
                        LineFrom = "Jędrzejewo",
                        LineTo = "Kaefer",
                        lineItems = data.GetRange(9,5),

                    },

                };

            });
            TimetableList = StableLineList;
        }
    }
}
