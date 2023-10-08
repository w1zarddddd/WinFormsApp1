using System;
using System.IO;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        private int count;
        private int start;
        private int end;
        private Random random = new Random();
        private string filePath;

        public Form2()
        {
            InitializeComponent();

            textBox1.TextChanged += TextBox1_TextChanged;
            textBox2.TextChanged += TextBox2_TextChanged;
            textBox3.TextChanged += TextBox3_TextChanged;

            //button1.Click += button1_Click;
            //button2.Click += button2_Click;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out count))
            {
                count = 0;
            }
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox2.Text, out start))
            {
                start = 0;
            }
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox3.Text, out end))
            {
                end = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (count <= 0)
            {
                MessageBox.Show("Введите корректное количество чисел.");
                return;
            }

            if (start > end)
            {
                MessageBox.Show("Начало диапазона должно быть меньше или равно концу диапазона.");
                return;
            }

            int[] numbers = new int[count];

            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Выберите файл для сохранения чисел.");
                return;
            }

            if (radioButton1.Checked)
            {
                for (int i = 0; i < count; i++)
                {
                    numbers[i] = start + i;
                }
            }
            else if (radioButton2.Checked)
            {
                for (int i = 0; i < count; i++)
                {
                    numbers[i] = end - i;
                }
            }
            else if (radioButton3.Checked)
            {
                for (int i = 0; i < count; i++)
                {
                    numbers[i] = random.Next(start, end + 1);
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    foreach (int num in numbers)
                    {
                        writer.WriteLine(num);
                    }
                }

                // После успешной генерации чисел, закрываем Form2
                this.Close();

                // Открываем Form1
                Form1 form1 = new Form1();
                form1.Show();

                MessageBox.Show($"Количество чисел: {count}, Диапазон: от {start} до {end}. Числа сгенерированы и сохранены в файле '{filePath}'.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении чисел: {ex.Message}");
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = saveFileDialog.FileName;
                    MessageBox.Show($"Выбран файл: {filePath}");
                }
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
