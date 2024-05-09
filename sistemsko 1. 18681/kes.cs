using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace sistemsko_1._18681
{
    public static class kes
    {
        private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<int, string> cache = new Dictionary<int, string>();

        public static string ReadFromCache(int key)
        {
            if(!cacheLock.IsReadLockHeld)
            cacheLock.EnterReadLock();
            if (cache.TryGetValue(key, out string value))
            {
                return value;
            }
            cacheLock.ExitReadLock();
            return "";
        }
        public static void WriteToCache(int key, string value)
        {
            if(cacheLock.IsReadLockHeld)
                cacheLock.ExitReadLock();
            cacheLock.EnterWriteLock();
            try
            {
                cache[key] = value;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                throw;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }
        public static bool WriteToCacheWithTimeout(int key, string value, int timeout)
        {
            if (cacheLock.TryEnterWriteLock(timeout))
            {
                try
                {
                    cache[key] = value;
                    return true;
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return false;
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            else
            {
                Console.WriteLine("Lock nije pribavljen u odgovarajucem intervalu.");
                return false;
            }
        }
        public static void RemoveFromCache(int key)
        {
            cacheLock.EnterWriteLock();
            try
            {
                //sta ako kljuc ne postoji?
                cache.Remove(key);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

    }
}

