using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blokus.Logic.MonteCarloTreeSearch;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Blokus.Logic.MCTS;

namespace TreeMerger
{
    public partial class Form1 : Form
    {
        Node resultTree;
        List<string> namesOfFiles;
        BinaryFormatter bf = new BinaryFormatter();

        public Form1()
        {
            InitializeComponent();
            resultTree = null;
            namesOfFiles = new List<string>();
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileNames.Count()>0)
            {
                foreach (string f in ofd.FileNames)
                {
                    namesOfFiles.Add(f);
                }
            }

        }

        private Node getTreeFromFile(string f)
        {
            FileStream fs = new FileStream(f, FileMode.Open);
            Node root = (Node)bf.Deserialize(fs);
            fs.Close();
            return root;
        }



        private void saveResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.OverwritePrompt = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
                bf.Serialize(fs, resultTree);
                fs.Close();
            }
        }

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (namesOfFiles.Count > 0 && resultTree==null)
            {
                resultTree = getTreeFromFile(namesOfFiles.ElementAt(0));
                namesOfFiles.RemoveAt(0);
            }
            foreach (string f in namesOfFiles)
            {
                TreeMerger.Merge(resultTree, getTreeFromFile(f));
            }
            MessageBox.Show("Trees merged", "SUCCESS", MessageBoxButtons.OK);
        }


    }
}
