/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

#region Usings

using System;

#endregion

namespace sones.Plugins.Index.ErrorHandling
{
    public sealed class IndexKeyExistsException : ASonesIndexException
    {
        #region Data

        public String Info { get; private set; }

        #endregion

        #region Constructor

		/// <summary>
		/// Initializes a new instance of the IndexKeyExistsException.
		/// </summary>
		/// <param name="myInfo"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public IndexKeyExistsException(String myInfo, Exception innerException = null) : base(innerException)
        {
            Info = myInfo;
        }

        #endregion

        public override string ToString()
        {
            return Info;
        }

    }
}
