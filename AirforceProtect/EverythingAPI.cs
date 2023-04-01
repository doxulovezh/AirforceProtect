using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AirforceProtect
{
    /// 

    ///  /// 
    public class EverythingAPI
    {
        #region Const
        const string EVERYTHING_DLL_NAME = "Everything64.dll";
        #endregion

        #region DllImport
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern int Everything_SetSearch(string lpSearchString);
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern void Everything_SetMatchPath(bool bEnable);
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern void Everything_SetMatchCase(bool bEnable);
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern void Everything_SetMatchWholeWord(bool bEnable);
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern void Everything_SetRegex(bool bEnable);
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern void Everything_SetMax(int dwMax);
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern void Everything_SetOffset(int dwOffset);

        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern bool Everything_GetMatchPath();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern bool Everything_GetMatchCase();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern bool Everything_GetMatchWholeWord();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern bool Everything_GetRegex();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern UInt32 Everything_GetMax();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern UInt32 Everything_GetOffset();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern string Everything_GetSearch();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern StateCode Everything_GetLastError();

        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern bool Everything_Query();

        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern void Everything_SortResultsByPath();

        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern int Everything_GetNumFileResults();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern int Everything_GetNumFolderResults();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern int Everything_GetNumResults();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern int Everything_GetTotFileResults();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern int Everything_GetTotFolderResults();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern int Everything_GetTotResults();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern bool Everything_IsVolumeResult(int nIndex);
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern bool Everything_IsFolderResult(int nIndex);
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern bool Everything_IsFileResult(int nIndex);
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern void Everything_GetResultFullPathName(int nIndex, StringBuilder lpString, int nMaxCount);
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern void Everything_Reset();
        [DllImport(EVERYTHING_DLL_NAME)]
        private static extern void Everything_SetSort(int num);
        #endregion

        #region Enum
        enum StateCode
        {
            OK,
            MemoryError,
            IPCError,
            RegisterClassExError,
            CreateWindowError,
            CreateThreadError,
            InvalidIndexError,
            InvalidCallError
        }
        #endregion

        #region Property

        /// 

        /// Gets or sets a value indicating whether [match path].
        /// 
        /// true if [match path]; otherwise, false.
        public Boolean MatchPath
        {
            get
            {
                return Everything_GetMatchPath();
            }
            set
            {
                Everything_SetMatchPath(value);
            }
        }

        /*
 EVERYTHING_SORT_NAME_ASCENDING                      (1)
EVERYTHING_SORT_NAME_DESCENDING                     (2)
EVERYTHING_SORT_PATH_ASCENDING                      (3)
EVERYTHING_SORT_PATH_DESCENDING                     (4)
EVERYTHING_SORT_SIZE_ASCENDING                      (5)
EVERYTHING_SORT_SIZE_DESCENDING                     (6)
EVERYTHING_SORT_EXTENSION_ASCENDING                 (7)
EVERYTHING_SORT_EXTENSION_DESCENDING                (8)
EVERYTHING_SORT_TYPE_NAME_ASCENDING                 (9)
EVERYTHING_SORT_TYPE_NAME_DESCENDING                (10)
EVERYTHING_SORT_DATE_CREATED_ASCENDING              (11)
EVERYTHING_SORT_DATE_CREATED_DESCENDING             (12)
EVERYTHING_SORT_DATE_MODIFIED_ASCENDING             (13)
EVERYTHING_SORT_DATE_MODIFIED_DESCENDING            (14)
EVERYTHING_SORT_ATTRIBUTES_ASCENDING                (15)
EVERYTHING_SORT_ATTRIBUTES_DESCENDING               (16)
EVERYTHING_SORT_FILE_LIST_FILENAME_ASCENDING        (17)
EVERYTHING_SORT_FILE_LIST_FILENAME_DESCENDING       (18)
EVERYTHING_SORT_RUN_COUNT_ASCENDING                 (19)
EVERYTHING_SORT_RUN_COUNT_DESCENDING                (20)
EVERYTHING_SORT_DATE_RECENTLY_CHANGED_ASCENDING     (21)
EVERYTHING_SORT_DATE_RECENTLY_CHANGED_DESCENDING    (22)
EVERYTHING_SORT_DATE_ACCESSED_ASCENDING             (23)
EVERYTHING_SORT_DATE_ACCESSED_DESCENDING            (24)
EVERYTHING_SORT_DATE_RUN_ASCENDING                  (25)
EVERYTHING_SORT_DATE_RUN_DESCENDING                 (26)
         */
        /// 

        /// Gets or sets a value indicating whether [match case].
        /// 
        /// true if [match case]; otherwise, false.
        public Boolean MatchCase
        {
            get
            {
                return Everything_GetMatchCase();
            }
            set
            {
                Everything_SetMatchCase(value);
            }
        }

        /// 

        /// Gets or sets a value indicating whether [match whole word].
        /// 
        /// true if [match whole word]; otherwise, false.
        public Boolean MatchWholeWord
        {
            get
            {
                return Everything_GetMatchWholeWord();
            }
            set
            {
                Everything_SetMatchWholeWord(value);
            }
        }

        /// 

        /// Gets or sets a value indicating whether [enable regex].
        /// 
        /// true if [enable regex]; otherwise, false.
        public Boolean EnableRegex
        {
            get
            {
                return Everything_GetRegex();
            }
            set
            {
                Everything_SetRegex(value);
            }
        }
        #endregion


        #region Public Method
        /// 

        /// Resets this instance.
        /// 
        public void Reset()
        {
            Everything_Reset();
        }

        /// 

        /// Searches the specified key word.
        /// 
        /// The key word.
        /// 
        public IEnumerable Search(string keyWord)
        {
            Everything_SetSort(6);
            return Search(keyWord, 0, int.MaxValue);
        }
        public IEnumerable Search(string keyWord,int Maxnum)
        {
            Everything_SetSort(6);
            return Search(keyWord, 0, Maxnum);
        }
        /// 

        /// Searches the specified key word.
        /// 
        /// The key word.
        /// The offset.
        /// The max count.
        /// 
        public IEnumerable Search(string keyWord, int offset, int maxCount)
        {
            if (string.IsNullOrEmpty(keyWord))
                throw new ArgumentNullException("keyWord");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");

            if (maxCount < 0)
                throw new ArgumentOutOfRangeException("maxCount");

            Everything_SetSearch(keyWord);
            Everything_SetOffset(offset);
            Everything_SetMax(maxCount);
            if (!Everything_Query())
            {
                switch (Everything_GetLastError())
                {
                    case StateCode.CreateThreadError:
                        throw new CreateThreadException();
                    case StateCode.CreateWindowError:
                        throw new CreateWindowException();
                    case StateCode.InvalidCallError:
                        throw new InvalidCallException();
                    case StateCode.InvalidIndexError:
                        throw new InvalidIndexException();
                    case StateCode.IPCError:
                        throw new IPCErrorException();
                    case StateCode.MemoryError:
                        throw new MemoryErrorException();
                    case StateCode.RegisterClassExError:
                        throw new RegisterClassExException();
                }
                yield break;
            }

            const int bufferSize = 256;
            StringBuilder buffer = new StringBuilder(bufferSize);
            for (int idx = 0; idx < Everything_GetNumResults(); ++idx)
            {
                Everything_GetResultFullPathName(idx, buffer, bufferSize);
                yield return buffer.ToString();
            }
        }
        #endregion
    }


    /// 

    ///  /// 
    public class MemoryErrorException : ApplicationException
    {
    }

    /// 

    ///  /// 
    public class IPCErrorException : ApplicationException
    {
    }

    /// 

    ///  /// 
    public class RegisterClassExException : ApplicationException
    {
    }

    /// 

    ///  /// 
    public class CreateWindowException : ApplicationException
    {
    }

    /// 

    ///  /// 
    public class CreateThreadException : ApplicationException
    {
    }

    /// 

    ///  /// 
    public class InvalidIndexException : ApplicationException
    {
    }

    /// 

    ///  /// 
    public class InvalidCallException : ApplicationException
    {
    }

}