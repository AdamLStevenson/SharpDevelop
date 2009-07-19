// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadTreeViewTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
						"    def InitializeComponent(self):\r\n" +
						"        treeNode1 = System.Windows.Forms.TreeNode()\r\n" +
						"        treeNode2 = System.Windows.Forms.TreeNode()\r\n" +
						"        treeNode3 = System.Windows.Forms.TreeNode()\r\n" +
						"        self._treeView1 = System.Windows.Forms.TreeView()\r\n" +
						"        self.SuspendLayout()\r\n" +
						"        # \r\n" +
						"        # treeView1\r\n" +
						"        # \r\n" +
						"        treeNode1.Name = \"RootNode0\"\r\n" +
						"        treeNode1.Text = \"RootNode0.Text\"\r\n" +
						"        treeNode1.Nodes.AddRange(System.Array[System.Windows.Forms.TreeNode](\r\n" +
						"            [treeNode2]))\r\n" +
						"        treeNode2.Name = \"ChildNode0\"\r\n" +
						"        treeNode2.Text = \"ChildNode0.Text\"\r\n" +
						"        treeNode2.Nodes.AddRange(System.Array[System.Windows.Forms.TreeNode](\r\n" +
						"            [treeNode3]))\r\n" +
						"        treeNode3.Name = \"ChildNode1\"\r\n" +
						"        treeNode3.Text = \"ChildNode1.Text\"\r\n" +
						"        self._treeView1.Location = System.Drawing.Point(0, 0)\r\n" +
						"        self._treeView1.Name = \"treeView1\"\r\n" +
						"        self._treeView1.Nodes.AddRange(System.Array[System.Windows.Forms.TreeNode](\r\n" +
						"            [treeNode1]))\r\n" +
						"        self._treeView1.Size = System.Drawing.Size(100, 100)\r\n" +
						"        self._treeView1.TabIndex = 0\r\n" +
						"        # \r\n" +
						"        # MainForm\r\n" +
						"        # \r\n" +
						"        self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
						"        self.Controls.Add(self._treeView1)\r\n" +
						"        self.Name = \"MainForm\"\r\n" +
						"        self.ResumeLayout(False)\r\n" +
						"        self.PerformLayout()\r\n";
			}
		}
		
		public TreeView TreeView {
			get { return Form.Controls[0] as TreeView; }
		}
		
		[Test]
		public void OneRootNode()
		{
			Assert.AreEqual(1, TreeView.Nodes.Count);
		}
		
		[Test]
		public void RootNodeHasOneChildNode()
		{
			Assert.AreEqual(1, TreeView.Nodes[0].Nodes.Count);
		}
		
		[Test]
		public void ChildNodeHasOneChildNode()
		{
			Assert.AreEqual(1, TreeView.Nodes[0].Nodes[0].Nodes.Count);
		}
	}
}