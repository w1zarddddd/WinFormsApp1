using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;

namespace WinFormsApp1
{
    public partial class Form3 : Form
    {
        private string filePath;
        private int[] originalArray;

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Выберите файл для сортировки.");
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Выбранный файл не существует.");
                return;
            }

            int[] array = File.ReadAllLines(filePath)
                .Select(line => int.Parse(line))
                .ToArray();

            originalArray = (int[])array.Clone(); // Сохраняем оригинальный массив

            Stopwatch stopwatch = new Stopwatch();
            int[] sortedArrayBlock;
            int comparisonsBlock = 0;
            int exchangesBlock = 0;

            int[] sortedArrayPatience;
            int comparisonsPatience = 0;
            int exchangesPatience = 0;

            stopwatch.Start();
            // Сортировка блочной сортировкой
            sortedArrayBlock = BlockSort(array, out comparisonsBlock, out exchangesBlock);
            stopwatch.Stop();
            TimeSpan elapsedTimeBlock = stopwatch.Elapsed;

            stopwatch.Restart();
            // Сортировка терпеливой сортировкой
            sortedArrayPatience = PatienceSort(array, out comparisonsPatience, out exchangesPatience);
            stopwatch.Stop();
            TimeSpan elapsedTimePatience = stopwatch.Elapsed;

            // Вывод результатов
            MessageBox.Show($"Сортировка блочной сортировкой:\n" +
                $"Время выполнения: {elapsedTimeBlock.TotalMilliseconds} мс\n" +
                $"Число сравнений: {comparisonsBlock}\n" +
                $"Число обменов: {exchangesBlock}\n\n" +
                $"Сортировка терпеливой сортировкой:\n" +
                $"Время выполнения: {elapsedTimePatience.TotalMilliseconds} мс\n" +
                $"Число сравнений: {comparisonsPatience}\n" +
                $"Число обменов: {exchangesPatience}");

            // Построение гистограмм
            BuildHistogram(comparisonsBlock, exchangesBlock, elapsedTimeBlock,
                comparisonsPatience, exchangesPatience, elapsedTimePatience);
            // Максимизируем окно формы
            this.WindowState = FormWindowState.Maximized;
        }

        private int[] BlockSort(int[] array, out int comparisons, out int exchanges)
        {
            comparisons = 0;
            exchanges = 0;

            // Реализация блочной сортировки (Bucket Sort)
            // Ваш код блочной сортировки здесь
            if (array.Length <= 1)
            {
                return array;
            }

            int min = array[0];
            int max = array[0];

            for (int i = 1; i < array.Length; i++)
            {
                comparisons++;
                if (array[i] < min)
                {
                    min = array[i];
                }
                else if (array[i] > max)
                {
                    max = array[i];
                }
            }

            int[] buckets = new int[max - min + 1];

            for (int i = 0; i < array.Length; i++)
            {
                buckets[array[i] - min]++;
            }

            int currentIndex = 0;

            for (int i = 0; i < buckets.Length; i++)
            {
                for (int j = 0; j < buckets[i]; j++)
                {
                    array[currentIndex] = i + min;
                    currentIndex++;
                    exchanges++;
                }
            }

            return array;
        }

        private int[] PatienceSort(int[] array, out int comparisons, out int exchanges)
        {
            comparisons = 0;
            exchanges = 0;

            // Реализация терпеливой сортировки (Patience Sort)
            // Ваш код терпеливой сортировки здесь
            if (array.Length <= 1)
            {
                return array;
            }

            List<List<int>> piles = new List<List<int>>();
            piles.Add(new List<int> { array[0] });

            for (int i = 1; i < array.Length; i++)
            {
                int currentElement = array[i];
                comparisons++;

                bool placed = false;

                for (int j = 0; j < piles.Count; j++)
                {
                    int topElement = piles[j].Last();
                    comparisons++;

                    if (currentElement >= topElement)
                    {
                        piles[j].Add(currentElement);
                        placed = true;
                        exchanges++;
                        break;
                    }
                }

                if (!placed)
                {
                    piles.Add(new List<int> { currentElement });
                }
            }

            List<int> sortedArray = new List<int>(array.Length);

            while (piles.Count > 0)
            {
                int minPileIndex = 0;

                for (int i = 1; i < piles.Count; i++)
                {
                    int topElement1 = piles[minPileIndex].Last();
                    int topElement2 = piles[i].Last();
                    comparisons++;

                    if (topElement2 < topElement1)
                    {
                        minPileIndex = i;
                    }
                }

                int topElement = piles[minPileIndex].Last();
                sortedArray.Add(topElement);
                piles[minPileIndex].RemoveAt(piles[minPileIndex].Count - 1);
                exchanges++;

                if (piles[minPileIndex].Count == 0)
                {
                    piles.RemoveAt(minPileIndex);
                }
            }

            return sortedArray.ToArray();
        }

        private void BuildHistogram(int comparisonsBlock, int exchangesBlock, TimeSpan elapsedTimeBlock,
                            int comparisonsPatience, int exchangesPatience, TimeSpan elapsedTimePatience)
        {
            chart1.Series.Clear();

            Series seriesComparisons = new Series
            {
                Name = "Сравнения",
                IsVisibleInLegend = true,
                ChartType = SeriesChartType.Column
            };
            seriesComparisons.Points.Add(comparisonsBlock);
            seriesComparisons.Points.Add(comparisonsPatience);

            Series seriesExchanges = new Series
            {
                Name = "Обмены",
                IsVisibleInLegend = true,
                ChartType = SeriesChartType.Column
            };
            seriesExchanges.Points.Add(exchangesBlock);
            seriesExchanges.Points.Add(exchangesPatience);

            Series seriesTime = new Series
            {
                Name = "Время (мс)",
                IsVisibleInLegend = true,
                ChartType = SeriesChartType.Column
            };
            seriesTime.Points.Add(elapsedTimeBlock.TotalMilliseconds);
            seriesTime.Points.Add(elapsedTimePatience.TotalMilliseconds);

            chart1.Series.Add(seriesComparisons);
            chart1.Series.Add(seriesExchanges);
            chart1.Series.Add(seriesTime);

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisX.Interval = 1; // Опционально, для лучшей видимости на гистограмме
        }




        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
