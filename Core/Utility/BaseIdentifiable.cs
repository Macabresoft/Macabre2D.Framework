﻿namespace Macabresoft.MonoGame.Core {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base implementation of <see cref="IIdentifiable" />.
    /// </summary>
    [DataContract]
    public class BaseIdentifiable : NotifyPropertyChanged, IIdentifiable {
        private Guid _id;

        /// <inheritdoc />
        [DataMember]
        public Guid Id {
            get {
                return this._id;
            }

            set {
                this.Set(ref this._id, value);
            }
        }
    }
}