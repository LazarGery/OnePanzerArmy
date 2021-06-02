using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class CoverTest
    {
        string _InitialScene;
        string _TestScene = "TestScene";

        // Waiting time for response in seconds
        float _TimeOut = 1.0f;
        float _ViewRange = 15;
        float _BuildingsRadius = 0.1f;
        Vector3 _Target_1 = new Vector3(-12.5f, 6.5f);
        Vector3 _Position_1 = new Vector3(-8.5f, 0.5f);
        Vector3 _Cover_1_Position = new Vector3(-10.5f, 1.5f);
        Vector3 _Cover_1_Front = new Vector3(-11.5f, 1.5f);
        Vector3 _InvalidPosition = new Vector3(0.5f, -0.5f);
        Vector3 _UnreachablePosition = new Vector3(-0.5f, -0.5f);

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
            CoverPoint cover = null;
            cover = GameController.Instance.Cover.GetCover(_Position_1, _InvalidPosition, _ViewRange, _BuildingsRadius);
            Assert.AreEqual(null, cover, "Working with invalid Target position");
            cover = GameController.Instance.Cover.GetCover(_Position_1, _UnreachablePosition, _ViewRange, _BuildingsRadius);
            Assert.AreEqual(null, cover, "Found cover for a position which absolutely has no chance for having a valid cover location");
        }

        [UnityTest]
        public IEnumerator FindCover()
        {
            float timer = 0;
            if (GameController.Instance == null && timer < _TimeOut)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            Assert.AreEqual(true, timer < _TimeOut, "Request Timed Out...");
            CoverPoint cover = null;
            cover = GameController.Instance.Cover.GetCover(_Position_1, _Target_1, _ViewRange, _BuildingsRadius);
            bool isSuccess = (cover != null);
            Assert.AreEqual(true, isSuccess, "Found no result when a cover position supposed to exist at (" + _Cover_1_Position.x + ";" + _Cover_1_Position.y + ")");
            isSuccess = (cover.Position == _Cover_1_Position && cover.Front == _Cover_1_Front);
            Assert.AreEqual(true, isSuccess, "Found the wrong cover position \n" +
                "awaited cover position = " + "(" + _Cover_1_Position.x + ";" + _Cover_1_Position.y + ") \n" +
                "awaited cover front = " + "(" + _Cover_1_Front.x + "; " + _Cover_1_Front.y + ") \n" +
                "gotten cover position = " + "(" + cover.Position.x + ";" + cover.Position.y + ") \n" +
                "gotten cover front = " + "(" + cover.Front.x + "; " + cover.Front.y + ")");
        }
    }
}
