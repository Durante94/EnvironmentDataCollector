﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnvironmentDataCollector
{
    public partial class EnvDataForm : System.Windows.Forms.Form
    {
        public EnvDataForm()
        {
            InitializeComponent();
        }

        //UPDATE DB DA FILE CARICATO
        private void FileBtn_Click(object sender, EventArgs e)
        {
            FileDialog.ShowDialog();
        }

        //RICERCA DATI PER FILTRO
        private void SearchBtn_Click(object sender, EventArgs e)
        {

        }

        //ESPORTA DATI RICERCA
        private void ExportBtn_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
