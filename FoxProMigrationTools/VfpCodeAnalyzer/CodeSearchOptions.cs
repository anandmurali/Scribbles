using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfpCodeAnalyzer
{
    public class CodeSearchOptions : ObservableBase
    {
        #region Properties
        public string SearchString { get; set; }

        public bool IsCaseSensitive { get; set; }


        private List<SearchFileDetail> _fileDetails;

        public List<SearchFileDetail> FileDetails
        {
            get { return _fileDetails; }
            set
            {
                _fileDetails = value; 
                OnPropertyChanged();
            }
        }
        
        

        private bool _searchInSelectedFiles;

        public bool SearchInSelectedFiles
        {
            get { return _searchInSelectedFiles; }
            set
            {
                _searchInSelectedFiles = value; 
                OnPropertyChanged();
            }
        }
        
        #endregion
    }

    public class SearchFileDetail : ObservableBase
    {
        #region Properties

        public string FilePath { get; set; }

        public string FileName { get; set; }

        private bool _isIncluded;

        public bool IsIncluded
        {
            get { return _isIncluded; }
            set
            {
                _isIncluded = value; 
                OnPropertyChanged();
            }
        }
        
        #endregion

        public override string ToString()
        {
            return FileName;
        }
    }
}
