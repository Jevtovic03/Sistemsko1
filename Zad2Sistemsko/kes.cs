using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zad2Sistemsko
{
    public static class kes
    {
        private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<int, LinkedListNode<KeyValuePair<int, string>>> cacheMap = new Dictionary<int, LinkedListNode<KeyValuePair<int, string>>>();
        private static readonly LinkedList<KeyValuePair<int, string>> cacheList = new LinkedList<KeyValuePair<int, string>>();
        private static readonly int maxCacheSize = 100; // maksimalna veličina keša

        public static string ReadFromCache(int key)
        {
            cacheLock.EnterReadLock();
            try
            {
                if (cacheMap.TryGetValue(key, out var node))
                {
                    // Premesti korišćeni čvor na početak liste da označi da je nedavno korišćen
                    cacheList.Remove(node);
                    cacheList.AddFirst(node);
                    return node.Value.Value;
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
            return string.Empty;
        }

        public static void WriteToCache(int key, string value)
        {
            cacheLock.EnterWriteLock();
            try
            {
                if (cacheMap.TryGetValue(key, out var existingNode))
                {
                    // Ažuriraj vrednost postojećeg čvora i premesti ga na početak
                    cacheList.Remove(existingNode);
                    cacheMap[key] = cacheList.AddFirst(new KeyValuePair<int, string>(key, value));
                }
                else
                {
                    if (cacheList.Count >= maxCacheSize)
                    {
                        // Ukloni najstariji čvor
                        var lastNode = cacheList.Last;
                        cacheMap.Remove(lastNode.Value.Key);
                        cacheList.RemoveLast();
                    }
                    // Dodaj novi čvor na početak
                    cacheMap[key] = cacheList.AddFirst(new KeyValuePair<int, string>(key, value));
                }
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
                    WriteToCache(key, value);
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
                if (cacheMap.TryGetValue(key, out var node))
                {
                    cacheList.Remove(node);
                    cacheMap.Remove(key);
                }
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
