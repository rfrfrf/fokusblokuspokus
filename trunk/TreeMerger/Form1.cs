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

namespace TreeMerger
{
    public partial class Form1 : Form
    {
        List<MultipleTreeNode> trees;
        MultipleTreeNode resultTree;

        public Form1()
        {
            InitializeComponent();
            trees = new List<MultipleTreeNode>();
            resultTree = null;
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileNames.Count()>0)
            {
                FileStream fs;
                BinaryFormatter bf = new BinaryFormatter();
                foreach (string f in ofd.FileNames)
                {
                    fs = new FileStream(f, FileMode.Open);
                    trees.Add((MultipleTreeNode)bf.Deserialize(fs));
                    fs.Close();
                }
            }

        }

        private void saveResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.OverwritePrompt = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, resultTree);
                fs.Close();
            }
        }

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trees.Count > 0 && resultTree==null)
            {
                resultTree = trees.ElementAt(0);
                trees.Remove(resultTree);
            }
            foreach (MultipleTreeNode node in trees)
            {
                Merge(ref resultTree, node);
            }
        }

        private void Merge(ref MultipleTreeNode resultTree, MultipleTreeNode node)
        {
            resultTree.visitCount += node.visitCount;
            resultTree.victoryCount += node.victoryCount;
            foreach (MultipleTreeNode child in node.childrenList)
            {
                if (!resultTree.childrenList.Exists(e => e.move.Equals(child.move)))
                {
                    resultTree.childrenList.Add(child);
                }
                else
                {
                    MultipleTreeNode pomchild = resultTree.childrenList.Find(e => e.move.Equals(child.move));
                    Merge(ref pomchild, child);
                }
            }
        }
    }
}
