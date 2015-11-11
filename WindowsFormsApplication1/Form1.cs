using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using Hl7.Fhir;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using System.Web.Script.Serialization;

namespace WindowsFormsApplication1
{
    
    public partial class Form1 : Form
    {
        Measurement CurrentMeasurement;
        Patient CurrentPatient;
        Device CurrentDevice;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSearchPatient_Click(object sender, EventArgs e)
        {
            searchStatus.Text = "Searching...";
            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            var query = new string[] { "family=" + searchFamName.Text };
            Bundle result = client.Search<Patient>(query);

            searchStatus.Text = "Got " + result.Entry.Count() + " records!";
            label1.Text = "";
            label2.Text = "";
            foreach (var entry in result.Entry)
            {
                Patient p = (Patient)entry.Resource;
                var patientFirst = p.Name.First().Text;
                var patientLast = p.Name.Last().Text;

                label1.Text = label1.Text + p.Id + "\r\n";
                label2.Text = label2.Text + p.BirthDate + "\r\n";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            createPatientStatus.Text = "Creating...";
            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            var pat = new Patient() {BirthDate = birthDate.Text, Name= new List<HumanName>()};
            pat.Name.Add(HumanName.ForFamily(lastName.Text).WithGiven(givenName.Text));
            client.Create(pat);
            createPatientStatus.Text = "Created!";

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            patientSelectStatus.Text = "Selecting...";
            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            var query = new string[] { "_id=" + selectPatientSearchText.Text.Trim() };
            Bundle result = client.Search<Patient>(query);

            CurrentPatient = ((Patient)result.Entry.FirstOrDefault().Resource);

            patID.Text = CurrentPatient.Id;
            patBirthday.Text = CurrentPatient.BirthDate;
            patientSelectStatus.Text = "Done!";

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            searchObsStatus.Text = "Retrieving...";
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://www.med-it.nl:3000");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            String measurementDate = searchObsDate.Text.Replace("-", "");
            HttpResponseMessage response = client.GetAsync("/api/measurements/" + measurementDate).Result; 
            
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                JavaScriptSerializer ser = new JavaScriptSerializer();
                var a = ser.Deserialize<List<Measurement>>(data);
                CurrentMeasurement = a[0];
                obsActiveTime.Text = CurrentMeasurement.m_active_time.ToString();
                obsAge.Text = CurrentMeasurement.age.ToString();
                obsAsleepTime.Text = CurrentMeasurement.s_asleep_time.ToString();
                obsAwake.Text = CurrentMeasurement.s_awake.ToString();
                obsAwakenings.Text = CurrentMeasurement.s_awakenings.ToString();
                obsAwakeTime.Text = CurrentMeasurement.s_awake_time.ToString();
                obsBedtime.Text = CurrentMeasurement.s_bedtime.ToString();
                obsBMR.Text = CurrentMeasurement.bmr.ToString();
                obsCalories.Text = CurrentMeasurement.m_calories.ToString();
                obsClinicalDeep.Text = CurrentMeasurement.s_clinical_deep.ToString();
                obsCount.Text = CurrentMeasurement.s_count.ToString();
                obsDate.Text = CurrentMeasurement.date;
                obsDistance.Text = CurrentMeasurement.m_distance.ToString();
                obsDuration.Text = CurrentMeasurement.m_inactive_time.ToString();
                obsHeight.Text = CurrentMeasurement.height.ToString();
                obsInactiveTime.Text = CurrentMeasurement.m_inactive_time.ToString();
                obsLcat.Text = CurrentMeasurement.m_lcat.ToString();
                obsLcit.Text = CurrentMeasurement.m_lcit.ToString();
                obsLight.Text = CurrentMeasurement.s_light.ToString();
                obsQuality.Text = CurrentMeasurement.s_quality.ToString();
                obsRem.Text = CurrentMeasurement.s_rem.ToString();
                obsSteps.Text = CurrentMeasurement.m_steps.ToString();
                obsTotalCalories.Text = CurrentMeasurement.m_total_calories.ToString();
                obsWeight.Text = CurrentMeasurement.weight.ToString();
                searchObsStatus.Text = "Done!";
            }
            else
            {
                searchObsStatus.Text = "Error: " + response.StatusCode;
            }
        }

        class Measurement
        {
            public String date { get; set; }
            public double age { get; set; }
            public double bmr { get; set; }
            public double height { get; set; }
            public int m_active_time { get; set; }
            public double m_calories { get; set; }
            public int m_distance { get; set; }
            public int m_inactive_time { get; set; }
            public int m_lcat { get; set; }
            public int m_lcit { get; set; }
            public int m_steps { get; set; }
            public double m_total_calories { get; set; }
            public int s_asleep_time { get; set; }
            public int s_awake { get; set; }
            public int s_awake_time { get; set; }
            public int s_awakenings { get; set; }
            public int s_bedtime { get; set; }
            public int s_clinical_deep { get; set; }
            public int s_count { get; set; }
            public int s_duration { get; set; }
            public int s_light { get; set; }
            public int s_quality { get; set; }
            public int s_rem { get; set; }
            public double weight { get; set; }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void searchObsDate_ValueChanged(object sender, EventArgs e)
        {
            searchObsStatus.Text = "Ready!";
        }

        private void birthDate_ValueChanged(object sender, EventArgs e)
        {
            createPatientStatus.Text = "Ready!";
        }

        private void givenName_TextChanged(object sender, EventArgs e)
        {
            createPatientStatus.Text = "Ready!";
        }

        private void lastName_TextChanged(object sender, EventArgs e)
        {
            createPatientStatus.Text = "Ready!";
        }

        private void selectPatientSearchText_TextChanged(object sender, EventArgs e)
        {
            patientSelectStatus.Text = "Ready!";
        }

        private void btnDeviceSelect_Click(object sender, EventArgs e)
        {
            deviceSelectStatus.Text = "Selecting...";
            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            var query = new string[] { "_id=" + selectDeviceSearch.Text.Trim() };
            Bundle result = client.Search<Device>(query);

            CurrentDevice = ((Device)result.Entry.FirstOrDefault().Resource);

            deviceID.Text = CurrentDevice.Id;
            deviceManufacturer.Text = CurrentDevice.Manufacturer;
            deviceModel.Text = CurrentDevice.Model;
            deviceSelectStatus.Text = "Done!";
        }

        private void btnDeviceCreate_Click(object sender, EventArgs e)
        {
            createDeviceStatus.Text = "Creating...";

            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            var dev = new Device() { Manufacturer = createDeviceManufacturer.Text, Model =  createDeviceModel.Text};
            client.Create(dev);

            createDeviceStatus.Text = "Done!";
        }

        private void btnDeviceSearch_Click(object sender, EventArgs e)
        {
            searchDeviceStatus.Text = "Searching...";

            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            var query = new string[] { "manufacturer=" + searchDeviceText.Text.Trim() };
            Bundle result = client.Search<Device>(query);

            searchDeviceStatus.Text = "Got " + result.Entry.Count() + " records!";

            deviceRes1.Text = "";
            deviceRes2.Text = "";
            deviceRes3.Text = "";
            foreach (var entry in result.Entry)
            {
                Device d = (Device)entry.Resource;

                deviceRes1.Text = deviceRes1.Text + d.Id + "\r\n";
                deviceRes2.Text = deviceRes2.Text + d.Manufacturer + "\r\n";
                deviceRes3.Text = deviceRes3.Text + d.Model + "\r\n";
            }

            searchDeviceStatus.Text = "Done!";
        }

    }
}
