using AvariModel.Model;
using AvariModel.Persistence;
using Moq;

namespace AvariTest
{
    [TestClass]
    public class AwariGameModelTest
    {


        private AwariGameModel model = null!; // a tesztelendõ modell
        private AwariTable mockedTable = null!; // mockolt játéktábla
        private Mock<AwariDataAcces> mock = null!; // az adatelérés mock-ja

        [TestInitialize]
        public void Initialize()
        {
            mockedTable = new AwariTable(8);
            // elõre definiálunk egy játéktáblát a perzisztencia mockolt teszteléséhez

            mock = new Mock<AwariDataAcces>();
            mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(mockedTable));
            // a mock a LoadAsync mûveletben bármilyen paraméterre az elõre beállított játéktáblát fogja visszaadni

            model = new AwariGameModel(mock.Object);
            // példányosítjuk a modellt a mock objektummal

            model.GameAdvanced += Model_GameAdvanced;
            model.GameOver += Model_GameOver;
        }

        [TestMethod]
        public void AwariGameModelNewGameMediumTest()
        {
            model.NewGame();

            Assert.AreEqual(MapSize.Medium, model.MapSize); // méretre jó

            int hatok = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (model.Table.GetValue(i, j) == 6)
                    {
                        hatok++;
                    }
                }
            }
            Assert.AreEqual(16, hatok);
        }

        [TestMethod]
        public void AwariGameModelNewGameSmallTest()
        {
            model.MapSize = MapSize.Small;
            model.NewGame();

            Assert.AreEqual(MapSize.Small, model.MapSize); // méretre jó

            int hatok = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (model.Table.GetValue(i, j) == 6)
                    {
                        hatok++;
                    }
                }
            }
            Assert.AreEqual(8, hatok);
        }

        [TestMethod]
        public void AwariGameModelNewGameLargeTest()
        {
            model.MapSize = MapSize.Large;
            model.NewGame();

            Assert.AreEqual(MapSize.Large, model.MapSize); // méretre jó

            int hatok = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (model.Table.GetValue(i, j) == 6)
                    {
                        hatok++;
                    }
                }
            }
            Assert.AreEqual(24, hatok);
        }

        [TestMethod]
        public void AwariGameModelStepTest()
        {
            model.NewGame();


            //A pályán a teszre releváns értékek 6-ok
            for (int j = 0; j < 8; j++)
            {
                Assert.AreEqual(6, model.Table[0, j]);
                Assert.AreEqual(6, model.Table[2, j]);

            }


            model.Step(0, 1);

            //Lenulláza az értéket
            Assert.AreEqual(0, model.Table[0, 1]);

            //Növeli az utána jövõt
            Assert.AreEqual(7, model.Table[0, 2]);

            //A mezõ lépés után zárolva van
            Assert.IsTrue(model.Table.IsLocked(0, 1));

            //A piros játékos tud lépni
            Assert.IsFalse(model.Table.IsLocked(2, 1));
        }

        [TestMethod]
        public void OneStepAwayFromVictory()
        {
            model.NewGame();


            //A pályán a teszre releváns értékek 6-ok
            for (int j = 0; j < 8; j++)
            {
                model.Table.SetValue(0, j, 0);
                model.Table.SetValue(2, j, 0);

            }

            model.Table.SetValue(0, 7, 2);
            model.Table.SetValue(2, 7, 1);


            model.Step(2, 7);

            //Tényleg mindenhol 0
            for (int j = 0; j < 8; j++)
            {
                Assert.AreEqual(0, model.Table[2, j]);

            }

            //Megnézzük hogy tényleg vége van-e
            Assert.IsTrue(model.IsGameOver);

        }

        [TestMethod]
        public async Task AwariGameModelLoadTest()
        {
            // kezdünk egy új játékot
            model.NewGame();

            // majd betöltünk egy játékot
            await model.LoadGameAsync(String.Empty);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Assert.AreEqual(mockedTable.GetValue(i, j), model.Table.GetValue(i, j));
                    // ellenõrizzük, valamennyi mezõ értéke megfelelõ-e
                    Assert.AreEqual(mockedTable.IsLocked(i, j), model.Table.IsLocked(i, j));
                    // ellenõrizzük, valamennyi mezõ zároltsága megfelelõ-e
                }
            // ellenõrizzük, hogy meghívták-e a Load mûveletet a megadott paraméterrel
            mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
        }

        private void Model_GameAdvanced(Object? sender, AwariEventArgs e)
        {

            Assert.AreEqual(model.Table.Sum(), model.IsGameOver); // megnézi, hogy vége van e

            //Assert.AreEqual(e.WhoWon, model.Table.StepValue); // a két értéknek egyeznie kell
        }

        private void Model_GameOver(object? sender, AwariEventArgs e)
        {
            Assert.IsTrue(model.IsGameOver); // biztosan vége van a játéknak

            Assert.AreEqual(e.WhoWon, "red");
        }
    }
}