using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using Grapute.Jobs;

namespace PipelinesLib
{
    /// <summary>
    /// Job data storage that stores data in the local file system.
    /// </summary>
    /// <seealso cref="IJobDataStorage" />
    public class FileSystemJobDataStorage : IJobDataStorage
    {
        private readonly string _basePath;
        private int _counter = 0;

        private string _idSuffix = null;

        private string IdSuffix
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_idSuffix))
                    _idSuffix = Assembly.GetEntryAssembly().GetName().Name;
                return _idSuffix;
            }
        }

        private readonly Dictionary<object, DataIdentifyer> _dataCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemJobDataStorage"/> class.
        /// </summary>
        /// <param name="basePath">The base path of the storage on the local disk.</param>
        public FileSystemJobDataStorage(string basePath)
        {
            _basePath = basePath;
            _dataCache = new Dictionary<object, DataIdentifyer>();
        }

        /// <summary>
        /// Generates an unique data identifyer.
        /// </summary>
        /// <returns></returns>
        public DataIdentifyer GenerateDataIdentifyer(object data)
        {
            DataIdentifyer id;

            if (_dataCache.ContainsKey(data))
            {
                id = _dataCache[data];
            }
            else
            {
                id = new DataIdentifyer {Id = $"df.{IdSuffix}_{_counter++}.bmp"};
                _dataCache.Add(data, id);
            }
            return id;
        }

        /// <summary>
        /// Obtains a data identifyed by the id from the storage.
        /// </summary>
        /// <typeparam name="T">The type of the obtained data.</typeparam>
        /// <param name="id">The data unique id.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T GetData<T>(DataIdentifyer id)
            where T : class
        {
            string fileName = Path.Combine(_basePath, id.Id);

            if (typeof(T) == typeof(Bitmap))
            {
                return (new Bitmap(fileName) as T);
            }

            if (typeof(T) == typeof(Bitmap[]))
            {
                var fileNames = File.ReadAllLines(fileName);
                var result = new Bitmap[fileNames.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    var bitmapFile = Path.Combine(_basePath, fileNames[i]);
                    result[i] = new Bitmap(bitmapFile);
                }

                return result as T;
            }

            if (typeof(T) == typeof(BitmapWithMarkup))
            {
                DataIdentifyer bmpid = new DataIdentifyer();
                var res = new BitmapWithMarkup();
                using (var stream = File.OpenRead(fileName))
                using (var r = new BinaryReader(stream))
                {
                    res.X = r.ReadInt32();
                    res.Y = r.ReadInt32();
                    res.Width = r.ReadInt32();
                    res.Height = r.ReadInt32();
                    bmpid.Id = r.ReadString();
                }

                res.Bmp = GetData<Bitmap>(bmpid);

                return res as T;
            }

            throw new NotImplementedException($"GetData for {typeof(T)} is not implemented.");
        }

        /// <summary>
        /// Saves the specifyed data in the storage.
        /// </summary>
        /// <typeparam name="T">The type of the saved data.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="id">The data unique identifyer in the storage.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool SaveData<T>(T data, DataIdentifyer id)
        {
            var filePath = Path.Combine(_basePath, id.Id);

            var bmp = data as Bitmap;
            if (bmp != null)
            {
                bmp.Save(filePath, ImageFormat.Bmp);
                return true;
            }

            var ids = data as DataIdentifyer[];
            if (ids != null)
            {
                // Usually an input data for a merge job that merges
                // a number of outputs that were recieved from other
                // jobs.
                // We save all the Ids of the outputs in a file.
                var sb = new StringBuilder();
                foreach (var x in ids)
                {
                    sb.AppendLine(x.Id);
                }
                File.AppendAllText(filePath, sb.ToString());
                return true;
            }

            var markedBitmap = data as BitmapWithMarkup;
            if (markedBitmap != null)
            {
                var bmpid = GenerateDataIdentifyer(markedBitmap.Bmp);
                SaveData<Bitmap>(markedBitmap.Bmp, bmpid);

                using (var stream = File.OpenWrite(filePath))
                using (var w = new BinaryWriter(stream))
                using (var ms = new MemoryStream())
                {
                    w.Write(markedBitmap.X);
                    w.Write(markedBitmap.Y);
                    w.Write(markedBitmap.Width);
                    w.Write(markedBitmap.Height);
                    w.Write(bmpid.Id);
                }

                return true;
            }

            throw new NotImplementedException($"SaveData for {typeof(T)} is not implemented.");
        }
    }
}