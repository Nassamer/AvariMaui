using AvariModel.Model;
using AvariModel.Persistence;
using Moq;

namespace AvariTest
{
    [TestClass]
    public class AwariGameModelTest
    {


        private AwariGameModel model = null!; // a tesztelend� modell
        private AwariTable mockedTable = null!; // mockolt j�t�kt�bla
        private Mock<AwariDataAcces> mock = null!; // az adatel�r�s mock-ja

        [TestInitialize]
        public void Initialize()
        {
            mockedTable = new AwariTable(8);
            // el�re defini�lunk egy j�t�kt�bl�t a perzisztencia mockolt tesztel�s�hez

            mock = new Mock<AwariDataAcces>();
            mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(mockedTable));
            // a mock a LoadAsync m�veletben b�rmilyen param�terre az el�re be�ll�tott j�t�kt�bl�t fogja visszaadni

            model = new AwariGameModel(mock.Object);
            // p�ld�nyos�tjuk a modellt a mock objektummal

            model.GameAdvanced += Model_GameAdvanced;
            model.GameOver += Model_GameOver;
        }

        [TestMethod]
        public void AwariGameModelNewGameMediumTest()
        {
            model.NewGame();

            Assert.AreEqual(MapSize.Medium, model.MapSize); // m�retre j�

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

            Assert.AreEqual(MapSize.Small, model.MapSize); // m�retre j�

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

            Assert.AreEqual(MapSize.Large, model.MapSize); // m�retre j�

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


            //A p�ly�n a teszre relev�ns �rt�kek 6-ok
            for (int j = 0; j < 8; j++)
            {
                Assert.AreEqual(6, model.Table[0, j]);
                Assert.AreEqual(6, model.Table[2, j]);

            }


            model.Step(0, 1);

            //Lenull�za az �rt�ket
            Assert.AreEqual(0, model.Table[0, 1]);

            //N�veli az ut�na j�v�t
            Assert.AreEqual(7, model.Table[0, 2]);

            //A mez� l�p�s ut�n z�rolva van
            Assert.IsTrue(model.Table.IsLocked(0, 1));

            //A piros j�t�kos tud l�pni
            Assert.IsFalse(model.Table.IsLocked(2, 1));
        }

        [TestMethod]
        public void OneStepAwayFromVictory()
        {
            model.NewGame();


            //A p�ly�n a teszre relev�ns �rt�kek 6-ok
            for (int j = 0; j < 8; j++)
            {
                model.Table.SetValue(0, j, 0);
                model.Table.SetValue(2, j, 0);

            }

            model.Table.SetValue(0, 7, 2);
            model.Table.SetValue(2, 7, 1);


            model.Step(2, 7);

            //T�nyleg mindenhol 0
            for (int j = 0; j < 8; j++)
            {
                Assert.AreEqual(0, model.Table[2, j]);

            }

            //Megn�zz�k hogy t�nyleg v�ge van-e
            Assert.IsTrue(model.IsGameOver);

        }

        [TestMethod]
        public async Task AwariGameModelLoadTest()
        {
            // kezd�nk egy �j j�t�kot
            model.NewGame();

            // majd bet�lt�nk egy j�t�kot
            await model.LoadGameAsync(String.Empty);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Assert.AreEqual(mockedTable.GetValue(i, j), model.Table.GetValue(i, j));
                    // ellen�rizz�k, valamennyi mez� �rt�ke megfelel�-e
                    Assert.AreEqual(mockedTable.IsLocked(i, j), model.Table.IsLocked(i, j));
                    // ellen�rizz�k, valamennyi mez� z�rolts�ga megfelel�-e
                }
            // ellen�rizz�k, hogy megh�vt�k-e a Load m�veletet a megadott param�terrel
            mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
        }

        private void Model_GameAdvanced(Object? sender, AwariEventArgs e)
        {

            Assert.AreEqual(model.Table.Sum(), model.IsGameOver); // megn�zi, hogy v�ge van e

            //Assert.AreEqual(e.WhoWon, model.Table.StepValue); // a k�t �rt�knek egyeznie kell
        }

        private void Model_GameOver(object? sender, AwariEventArgs e)
        {
            Assert.IsTrue(model.IsGameOver); // biztosan v�ge van a j�t�knak

            Assert.AreEqual(e.WhoWon, "red");
        }
    }
}