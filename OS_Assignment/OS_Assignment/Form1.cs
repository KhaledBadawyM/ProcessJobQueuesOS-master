using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace OS_Assignment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DataTable table = new DataTable();

        List<Job_Queue> JobDataList = new List<Job_Queue>();
        List<string> processNameList = new List<string>();
        Dictionary<int, List<Job_Queue>> Map = new Dictionary<int, List<Job_Queue>>();
        bool priortyFlag = false;
        bool FCFSFlag = false;
        bool SJFFlag = false;
        bool RoundFlag = false;

        bool preempriveFlag = false;

        private void DGV_QUEUES_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //ProcessListBox.DataSource = processNameList;
            table.Columns.Add("Process Name", typeof(string));
            table.Columns.Add("Duration", typeof(int));
            //DGV_QUEUES.DataSource = table;


            Preemptive_Button.Enabled = false;
            NonPreem_Button.Enabled = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FCFS_Button_Click(object sender, EventArgs e)
        {
            SJF_Button.Enabled = false;
            Priority_Button.Enabled = false;
            RoundRobin_Button.Enabled = false;
            PriNumber_Text.Enabled = false;
            SliceTime_Text.Enabled = false;
            FCFSFlag = true;
        }

        private void SJF_Button_Click(object sender, EventArgs e)
        {
            FCFS_Button.Enabled = false;
            Priority_Button.Enabled = false;
            RoundRobin_Button.Enabled = false;
            Preemptive_Button.Enabled = true;
            NonPreem_Button.Enabled = true;
            PriNumber_Text.Enabled = false;
            SliceTime_Text.Enabled = false;
            SJFFlag = true;
        }

        private void Priority_Button_Click(object sender, EventArgs e)
        {
            FCFS_Button.Enabled = false;
            SJF_Button.Enabled = false;
            RoundRobin_Button.Enabled = false;
            Preemptive_Button.Enabled = true;
            NonPreem_Button.Enabled = true;
            SliceTime_Text.Enabled = false;
            priortyFlag = true;
        }

        private void RoundRobin_Button_Click(object sender, EventArgs e)
        {
            FCFS_Button.Enabled = false;
            Priority_Button.Enabled = false;
            SJF_Button.Enabled = false;
            PriNumber_Text.Enabled = false;
            RoundFlag = true;
        }

        private void AddProc_Button_Click(object sender, EventArgs e)
        {
            Job_Queue J = new Job_Queue();
            if (priortyFlag)
            {
                J.Job_Queue_priority(procName_text.Text, int.Parse(ArrivalTime_text.Text), int.Parse(BurstTime_Text.Text), int.Parse(PriNumber_Text.Text));
                JobDataList.Add(J);
                if (Map.ContainsKey(J.arrivalTime))
                {
                    Map[J.arrivalTime].Add(J);
                }
                else
                {
                    Map.Add(J.arrivalTime, new List<Job_Queue> { J });
                }
            }
            else if(RoundFlag) {

                J.Job_Queue_Round(procName_text.Text, int.Parse(ArrivalTime_text.Text), int.Parse(BurstTime_Text.Text) , int.Parse(SliceTime_Text.Text));
                JobDataList.Add(J);
                
                if (Map.ContainsKey(J.arrivalTime))
                {
                    Map[J.arrivalTime].Add(J);
                }
                else
                {
                    Map.Add(J.arrivalTime, new List<Job_Queue> { J });
                }

            }
            else {

                J.Job_Queue_non_priority(procName_text.Text, int.Parse(ArrivalTime_text.Text), int.Parse(BurstTime_Text.Text));
                JobDataList.Add(J);
                //  Map[J.arrivalTime].Add(J);
                // Map.Add(J.arrivalTime

                if (Map.ContainsKey(J.arrivalTime))
                {
                    Map[J.arrivalTime].Add(J);
                }
                else
                {
                    Map.Add(J.arrivalTime, new List<Job_Queue> { J });
                }

            }
            //processNameList.Add(procName_text.Text);
            //ProcessListBox.Update();
            //ProcessListBox.Refresh();

        }

        private void Finish_Button_Click(object sender, EventArgs e)
        {
         
            Processing();
        }

        private void NonPreem_Button_Click(object sender, EventArgs e)
        {
            Preemptive_Button.Enabled = false; 
        }

        private void Preemptive_Button_Click(object sender, EventArgs e)
        {
            preempriveFlag = true;
            NonPreem_Button.Enabled = false; 
        }

        private void Processing()
        {
            if (FCFSFlag) FCFSProcess();
            else if (SJFFlag) SJFProcess();
            else if (priortyFlag) PriorityProcess();
            else if (RoundFlag) RoundProcess();
        }

        private void FCFSProcess()
        {
            int watingTime = 0 ;float avaregeTime;
            JobDataList.Sort(new Arrival_Sort());
            int time = 0; 
            foreach (var job in JobDataList)
            {
                watingTime += time - job.arrivalTime;
                table.Rows.Add(job.name, time+=job.burstTime);
            }
            avaregeTime = (float)watingTime / JobDataList.Count;
            av_text.Text = avaregeTime.ToString();
            DGV_QUEUES.DataSource = table;
            DGV_QUEUES.Update();
            DGV_QUEUES.Refresh();
        }

        private void SJFProcess()
        {
            if (preempriveFlag) SJFProcess_Preemptive();

            else
                SJFProcess_NonPreemptive();

        }


        private void PriorityProcess() {
            if (preempriveFlag) PrProcess_Preemptive();

            else
                PrProcess_NonPreemptive();

        }

        private void RoundProcess() {
            int watingTime=0;float avaregeTime;   
            List<int> keys = new List<int>();
            List<Job_Queue> watingList = new List<Job_Queue>();
            int time = 0;int i =0;
            int count = 1;Boolean flag = false; 
            foreach (var item in Map.Keys)
            {
                keys.Add(item);
            }
            foreach (var item in Map.Values)
            {
                
                watingList.AddRange(item);
                while (watingList.Count!=0) {
                    if (flag) { flag = false; i++; }
                    else i = 0;
                    for ( ;i<watingList.Count;i++)
                    {
                        var job = watingList[i];
                        watingTime += time - job.arrivalTime;

                        if (job.burstTime > job.sliceTime)
                        {
                            table.Rows.Add(job.name, time += job.sliceTime);
                            job.burstTime -= job.sliceTime;
                        }
                        else
                        {
                            table.Rows.Add(job.name, time += job.burstTime);
                            watingList.RemoveAt(i);
                            i--;
                        }
                        
                        if ((count < keys.Count && time >= keys[count]))
                        {
                            count++;flag = true; 
                            break;
                        }
                    }
                    if (flag) break;
                }
            }
            avaregeTime = (float)watingTime / JobDataList.Count;
            av_text.Text = avaregeTime.ToString();
            DGV_QUEUES.DataSource = table;
            DGV_QUEUES.Update();
            DGV_QUEUES.Refresh();

        }

        private void SJFProcess_NonPreemptive()
        {
            int watingTime = 0; float avaregeTime;
            List<int> keys = new List<int>();
            List<Job_Queue> watingList = new List<Job_Queue>();
            int time = 0;
            int count = 1;
            foreach (var item in Map.Keys)
            {
                keys.Add(item);
            }
            foreach (var item in Map.Values)
            {
                watingList.AddRange(item);
                watingList.Sort(new Short_Sort());
                foreach (var job in watingList)
                {
                    watingTime += time - job.arrivalTime;
                    table.Rows.Add(job.name, time += job.burstTime);
                    
                    if (count < keys.Count && time >= keys[count])
                    {
                        count++;
                        break;
                    }
                }
            }
            avaregeTime = (float)watingTime / JobDataList.Count;
            av_text.Text = avaregeTime.ToString();
            DGV_QUEUES.DataSource = table;
            DGV_QUEUES.Update();
            DGV_QUEUES.Refresh();

        }

        private void SJFProcess_Preemptive()
        {
            int watingTime = 0; float avaregeTime;int D=0; 
            List<int> keys = new List<int>();
            List<Job_Queue> watingList = new List<Job_Queue>();
            List<printedList> PList = new List<printedList>();
            int time = 0;
           // int count = 1;
            
            foreach (var item in Map.Keys)
            {
                keys.Add(item);
            }
            printedList pl = new printedList() ; 
            for (int i = 0; i <= keys[keys.Count - 1]; i++)
            {
                if (Map.ContainsKey(i))
                {
                    watingList.AddRange(Map[i]);
                    watingList.Sort(new Short_Sort());
                }
                
                if (i == 0) { 
                    pl.name = watingList[i].name;
                    pl.duration = i+1;
                
                }
                else
                {
                    if (pl.name == watingList[0].name)
                    {
                        pl.duration += 1;
                        
                    }
                    else
                    {
                        PList.Add(pl);
                        watingTime += pl.duration - watingList[0].arrivalTime;
                        pl.name = watingList[0].name;
                        pl.duration = i+1;
                        
                    }
                }
                watingList[0].burstTime -= 1;
                if (watingList[0].burstTime == 0)
                    watingList.RemoveAt(0);
            }

            foreach (var i in PList)
            {
                
                table.Rows.Add(i.name, i.duration);
            }

            time = keys[keys.Count - 1]+1;
            watingList.Sort(new Short_Sort());
            foreach (var job in watingList)
            {
                watingTime += time - job.arrivalTime;
                table.Rows.Add(job.name, time += job.burstTime);
            }
            avaregeTime = ((float)watingTime / JobDataList.Count)-1;
            av_text.Text = avaregeTime.ToString();
            DGV_QUEUES.DataSource = table;
            DGV_QUEUES.Update();
            DGV_QUEUES.Refresh();

        }

        private void PrProcess_NonPreemptive()
        {
            int watingTime = 0; float avaregeTime;
            List<int> keys = new List<int>();
            List<Job_Queue> watingList = new List<Job_Queue>();
            int time = 0;
            int count = 1;
            foreach (var item in Map.Keys)
            {
                keys.Add(item);
            }
            foreach (var item in Map.Values)
            {
                watingList.AddRange(item);
                watingList.Sort(new Priority_Sort());
                foreach (var job in watingList)
                {
                    watingTime += time - watingList[0].arrivalTime;
                    table.Rows.Add(job.name, time += job.burstTime);

                    if (count < keys.Count && time >= keys[count])
                    {
                        count++;
                        break;
                    }
                }
            }
            avaregeTime = (float)watingTime / JobDataList.Count;
            av_text.Text = avaregeTime.ToString();
            DGV_QUEUES.DataSource = table;
            DGV_QUEUES.Update();
            DGV_QUEUES.Refresh();

        }
        private void PrProcess_Preemptive()
        {
            int watingTime = 0; float avaregeTime = 0; 
            List<int> keys = new List<int>();
            List<Job_Queue> watingList = new List<Job_Queue>();
            List<printedList> PList = new List<printedList>();
            int time = 0;
            // int count = 1;

            foreach (var item in Map.Keys)
            {
                keys.Add(item);
            }
            printedList pl = new printedList();
            for (int i = 0; i <= keys[keys.Count - 1]; i++)
            {
                if (Map.ContainsKey(i))
                {
                    watingList.AddRange(Map[i]);
                    watingList.Sort(new Priority_Sort());
                }
               // watingTime += time - watingList[0].arrivalTime;
                if (i == 0)
                {
                    pl.name = watingList[i].name;
                    pl.duration = i + 1;

                }
                else
                {
                    if (pl.name == watingList[0].name)
                    {
                        pl.duration += 1;

                    }
                    else
                    {
                        PList.Add(pl);
                        watingTime += pl.duration - watingList[0].arrivalTime;
                        pl.name = watingList[0].name;
                        pl.duration = i + 1;

                    }
                }
                watingList[0].burstTime -= 1;
                if (watingList[0].burstTime == 0)
                    watingList.RemoveAt(0);
            }

            foreach (var i in PList)
            {
                table.Rows.Add(i.name, i.duration);
            }

            time = keys[keys.Count - 1] + 1;
            watingList.Sort(new Priority_Sort());
            foreach (var job in watingList)
            {
                watingTime += time - job.arrivalTime;
                table.Rows.Add(job.name, time += job.burstTime);
            }
            avaregeTime = (float)watingTime / JobDataList.Count;
            av_text.Text = avaregeTime.ToString();
            DGV_QUEUES.DataSource = table;
            DGV_QUEUES.Update();
            DGV_QUEUES.Refresh();
        }

        //private void reload_botton_click(object sender, eventargs e)
        //{
            
        //}


    }
}
