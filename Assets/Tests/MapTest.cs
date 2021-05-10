using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class MapTest
    {
        string _InitialScene;
        string _TestScene = "TestScene";

        //Waiting time for response in seconds
        float _TimeOut = 1.0f;

        // The Unity's vector system defines the 2 dimension coordinates as
        // [column, row ~ numbering starts from the left bottom corner]
        // In summary it can be symbolized as a 90 degree clock-wise rotation between the usual array approach
        // [row, column ~ numbering starts from the left upper place] approach
        int _OffsetX = 14;
        int _OffsetY = 14;
        char _WalkableFieldCharacter = '0';
        char _NonWalkableFieldCharacter = 'X';
        string[] _MapLayout = {"XXXXXXXXXXXXXXXXXXXXXXXXXXX",
                               "X00000000000000000000XXX00X",
                               "X00000000000000000000XXX00X",
                               "X00XXXXXXXX00000XXXXXXXX00X",
                               "X00XXXXXXXX00000XXXXXXXX00X",
                               "X00XXXXXXXX00000XXXXXXXX00X",
                               "X00XXXXXXXX00000XXXXXXXX00X",
                               "X00XXXX0000000000000XXXX00X",
                               "X00XXXX0000000000000XXXX00X",
                               "X00XXXX0000000000000XXXX00X",
                               "X00XXXX000XXXXXXX000XXXX00X",
                               "X000000000XXXXXXX000000000X",
                               "X000000000XXXXXXX000000000X",
                               "X000000000XXX0XXX000000000X",
                               "X000000000XXXXXXX000000000X",
                               "X000000000XXXXXXX000000000X",
                               "X00XXXX000XXXXXXX000XXXX00X",
                               "X00XXXX0000000000000XXXX00X",
                               "X00XXXX0000000000000XXXX00X",
                               "X00XXXX0000000000000XXXX00X",
                               "X00XXXXXXXX00000XXXXXXXX00X",
                               "XXXXXXXXXXX00000XXXXXXXX00X",
                               "XXXXXXXXXXX00000XXXXXXXX00X",
                               "XXXXXXXXXXX00000XXXXXXXX00X",
                               "X0000000000000000000000000X",
                               "X0000000000000000000000000X",
                               "XXXXXXXXXXXXXXXXXXXXXXXXXXX"};

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
        public IEnumerator MapTranslation()
        {
            float timer = 0;
            if (GameController.Instance == null && timer < _TimeOut)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            Assert.AreEqual(true, timer < _TimeOut, "Request Timed Out...");
            MapGrid map = GameController.Instance.Map;
            Assert.AreEqual(_OffsetX, map.Offset_X, "The Offset value of the X coordinate doesn't match");
            Assert.AreEqual(_OffsetY, map.Offset_Y, "The Offset value of the Y coordinate doesn't match");
            Assert.AreEqual(map.Tiles.GetLength(0), _MapLayout.Length, "The array's size belonging to the X coordinate doesn't match");
            Assert.AreEqual(map.Tiles.GetLength(1), _MapLayout[0].Length, "The array's size belonging to the Y coordinate doesn't match");
            bool result = true;
            int x = 0;
            int y = 0;
            string actualRow = string.Empty;
            while (result && x < map.Tiles.GetLength(0))
            {
                y = 0;
                actualRow = _MapLayout[x];
                while (result && y < map.Tiles.GetLength(1))
                {
                    if ((map.Tiles[x, y] && actualRow[y] != _WalkableFieldCharacter) ||
                        (!map.Tiles[x, y]) && actualRow[y] != _NonWalkableFieldCharacter)
                    {
                        result = false;
                        y -= 1;
                        x -= 1;
                    }
                    y += 1;
                }
                x += 1;
            }
            Assert.AreEqual(true, result, "Map translation into 2 dimension walkable/nonwalkable array failed" +
                                          "at position [" + x + ", " + y + "] " +
                                          "which translates from [" + (x - _OffsetX) + ", " + (y - _OffsetY) + "] field on the map");
        }
    }
}
