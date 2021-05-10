using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    // MapTest must be successful before this one
    public class PathFindTest
    {
        string _InitialScene;
        string _TestScene = "TestScene";
        
        // Waiting time for response in seconds
        float _TimeOut = 1.0f;
        Vector3 _StartPosition = new Vector3(-12.5f, -12.5f);
        Vector3 _InvalidPosition = new Vector3(0.5f, -0.5f);
        Vector3 _UnreachablePosition = new Vector3(-0.5f, -0.5f);

        Vector3 _EasyDestination = new Vector3(-0.5f, 8.5f);
        int _nEasyDestination = 22;
        
        Vector3 _Destination = new Vector3(-12.5f, 11.5f);
        int _nDestination = 34;

        [OneTimeSetUp]
        public void Setup()
        {
            _InitialScene = SceneManager.GetActiveScene().name;
            if (_InitialScene != _TestScene)
            {
                SceneManager.LoadScene(_TestScene);
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (_InitialScene != _TestScene)
            {
                SceneManager.LoadScene(_InitialScene);
            }
        }

        [UnityTest]
        public IEnumerator Invalid_Parameters()
        {
            float timer = 0;
            if (GameController.Instance == null && timer < _TimeOut)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            Assert.AreEqual(true, timer < _TimeOut, "Request Timed Out...");
            List<Vector2Int> path;
            path = GameController.Instance.PathFinder.FindPath(_StartPosition, _InvalidPosition);
            Assert.AreEqual(null, path, "Working with invalid Destination position");
            path = GameController.Instance.PathFinder.FindPath(_InvalidPosition, _StartPosition);
            Assert.AreEqual(null, path, "Working with invalid Start position");
            path = GameController.Instance.PathFinder.FindPath(_StartPosition, _StartPosition);
            Assert.AreEqual(null, path, "Working when the Start and Destination positions are the same");
        }

        [UnityTest]
        public IEnumerator Unreachable_Destination()
        {
            float timer = 0;
            if (GameController.Instance == null && timer < _TimeOut)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            Assert.AreEqual(true, timer < _TimeOut, "Request Timed Out...");
            List<Vector2Int> path;
            path = GameController.Instance.PathFinder.FindPath(_StartPosition, _UnreachablePosition);
            Assert.AreNotEqual(null, path, "Expected value: List of 0 elements | Received value: null reference");
            Assert.AreEqual(0, path.Count, "Found a way for an unreachable Destination");
        }

        [UnityTest]
        public IEnumerator Straightforward_Destination()
        {
            float timer = 0;
            if (GameController.Instance == null && timer < _TimeOut)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            Assert.AreEqual(true, timer < _TimeOut, "Request Timed Out...");
            List<Vector2Int> path;
            path = GameController.Instance.PathFinder.FindPath(_StartPosition, _EasyDestination);
            Assert.AreNotEqual(null, path, "Expected value: List of steps | Received value: null reference");
            Assert.AreEqual(_nEasyDestination, path.Count, "The number of steps to reach the Destination mismatch with the optimal value");
            CheckRoute(path);
        }

        [UnityTest]
        public IEnumerator Tricky_Destination()
        {
            float timer = 0;
            if (GameController.Instance == null && timer < _TimeOut)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            Assert.AreEqual(true, timer < _TimeOut, "Request Timed Out...");
            List<Vector2Int> path;
            path = GameController.Instance.PathFinder.FindPath(_StartPosition, _Destination);
            Assert.AreNotEqual(null, path, "Expected value: List of steps | Received value: null reference");
            Assert.AreEqual(_nDestination, path.Count, "The number of steps to reach the Destination mismatch with the optimal value");
            CheckRoute(path);
        }

        void CheckRoute(List<Vector2Int> Path)
        {
            MapGrid map = GameController.Instance.Map;
            int index = 0;
            bool isWalkable = true;
            while (isWalkable && index < Path.Count)
            {
                if (!map.Tiles[Path[index].x + map.Offset_X, Path[index].y + map.Offset_Y] ||
                    index > 0 && Path[index].x != Path[index - 1].x && Path[index].y != Path[index - 1].y &&
                   (!map.Tiles[Path[index].x + map.Offset_X, Path[index - 1].y + map.Offset_Y] ||
                    !map.Tiles[Path[index - 1].x + map.Offset_X, Path[index].y + map.Offset_Y]))
                {
                    isWalkable = false;
                }
                index += 1;
            }
            Assert.AreEqual(Path.Count, index, "On the calculated route the " + (index - 1) + "th step is blocked by unwalkable fields");
        }
    }
}
