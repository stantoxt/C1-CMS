﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.C1Console.Security;
using Composite.Data;


namespace Composite.C1Console.Trees
{
    /// <summary>    
    /// </summary>
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    public sealed class PiggybagDataFinder
    {
        private List<EntityToken> _parentEntityTokens;

        private readonly Dictionary<string, string> _piggybag;
        private readonly EntityToken _currentEntityToken;



        /// <exclude />
        public PiggybagDataFinder(Dictionary<string, string> piggybag, EntityToken currentEntityToken)
        {
            _piggybag = piggybag;
            _currentEntityToken = currentEntityToken;
        }


         
        /// <exclude />
        public Dictionary<string, string> Piggybag
        {
            get
            {
                return _piggybag;
            }
        }



        /// <exclude />
        public IData GetData(Type interfaceType, IData currentData = null)
        {
            if ((currentData != null) && (currentData.DataSourceId.InterfaceType == interfaceType)) return currentData;

            DataEntityToken dataEntityToken = GetDataEntityToken(interfaceType);

            return dataEntityToken.Data;
        }



        /// <exclude />
        public IData TryGetData(Type interfaceType, IData currentData = null)
        {
            if ((currentData != null) && (currentData.DataSourceId.InterfaceType == interfaceType)) return currentData;

            DataEntityToken dataEntityToken = TryGetDataEntityToken(interfaceType);            
            if (dataEntityToken == null) return null;

            return dataEntityToken.Data;
        }




        private DataEntityToken GetDataEntityToken(Type interfaceType)
        {
            DataEntityToken dataEntityToken = TryGetDataEntityToken(interfaceType);

            if ((dataEntityToken == null) || (dataEntityToken.InterfaceType != interfaceType)) throw new InvalidOperationException("Could not find data entity token that match the given interface type");            

            return dataEntityToken;
        }



        private DataEntityToken TryGetDataEntityToken(Type interfaceType)
        {
            DataEntityToken dataEntityToken = _currentEntityToken as DataEntityToken;
            if ((dataEntityToken == null) || (dataEntityToken.InterfaceType != interfaceType))
            {
                dataEntityToken = this.ParentEntityTokens.FindDataEntityToken(interfaceType);
            }            

            return dataEntityToken;
        }



        private IEnumerable<EntityToken> ParentEntityTokens
        {
            get
            {
                if (_parentEntityTokens == null)
                {
                    _parentEntityTokens = _piggybag.GetParentEntityTokens().ToList();
                }

                return _parentEntityTokens;
            }
        }
    }
}
