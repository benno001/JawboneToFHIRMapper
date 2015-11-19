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
        // Fields to track the selected measurement, patient and device
        Measurement CurrentMeasurement;
        Patient CurrentPatient;
        Device CurrentDevice;

        public Form1()
        {
            InitializeComponent();
        }

        // When btnSearchPatient is clicked, search patient
        private void btnSearchPatient_Click(object sender, EventArgs e)
        {
            // Edit status text
            searchStatus.Text = "Searching...";
            
            // Set FHIR endpoint and create client
            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            // Search endpoint with a family name, input from searchFamName
            var query = new string[] { "family=" + searchFamName.Text };
            Bundle result = client.Search<Patient>(query);

            // Edit status text
            searchStatus.Text = "Got " + result.Entry.Count() + " records!";
            
            // Clear results labels
            label1.Text = "";
            label2.Text = "";

            // For every patient in the result, add name and ID to a label
            foreach (var entry in result.Entry)
            {
                Patient p = (Patient)entry.Resource;
                label1.Text = label1.Text + p.Id + "\r\n";
                label2.Text = label2.Text + p.BirthDate + "\r\n";
            }
        }

        // When button 2 is clikced, create patient
        private void button2_Click(object sender, EventArgs e)
        {
            // Edit status text
            createPatientStatus.Text = "Creating...";

            // Set FHIR endpoint and create client
            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            // Create new patient birthdate and name
            var pat = new Patient() {BirthDate = birthDate.Text, Name= new List<HumanName>()};
            pat.Name.Add(HumanName.ForFamily(lastName.Text).WithGiven(givenName.Text));

            // Upload to server
            client.Create(pat);
            createPatientStatus.Text = "Created!";

        }

        // When button 1 is clicked, select patient
        private void button1_Click_1(object sender, EventArgs e)
        {
            // Edit status text
            patientSelectStatus.Text = "Selecting...";

            // Set FHIR endpoint and create client
            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            // Search patient based on ID from user input
            var query = new string[] { "_id=" + selectPatientSearchText.Text.Trim() };
            Bundle result = client.Search<Patient>(query);

            // Set the current patient
            CurrentPatient = ((Patient)result.Entry.FirstOrDefault().Resource);

            // Edit selected patient text
            patID.Text = CurrentPatient.Id;
            patBirthday.Text = CurrentPatient.BirthDate;
            
            // Edit status text
            patientSelectStatus.Text = "Done!";

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        // When button1 is clicked, retrieve measurements
        private void button1_Click(object sender, EventArgs e)
        {
            searchObsStatus.Text = "Retrieving...";

            // Connect to RESTful server
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://www.med-it.nl:3000");

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")); // Set output to json
            String measurementDate = searchObsDate.Text.Replace("-", ""); // Clean up date string

            // Get response
            HttpResponseMessage response = client.GetAsync("/api/measurements/" + measurementDate).Result; 
            
            // If response is successful, select measurement
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                JavaScriptSerializer ser = new JavaScriptSerializer();
                var measurement = ser.Deserialize<List<Measurement>>(data);
                CurrentMeasurement = measurement[0];
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
                // If not successful, send error code to label
                searchObsStatus.Text = "Error: " + response.StatusCode;
            }
        }

        /*
         * Class for measurement received from server
         * */
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

        // Select a device
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

        // Create a device
        private void btnDeviceCreate_Click(object sender, EventArgs e)
        {
            createDeviceStatus.Text = "Creating...";

            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            var dev = new Device() { Manufacturer = createDeviceManufacturer.Text, Model =  createDeviceModel.Text};
            client.Create(dev);

            createDeviceStatus.Text = "Done!";
        }

        // Search for a device
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
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            // Create new value for observation
            SampledData val = new SampledData();
            CodeableConcept concept = new CodeableConcept();
            concept.Coding = new List<Coding>();

            if (conversionList.Text.Equals("age")) 
            {
                val.Data = CurrentMeasurement.age.ToString();
                Coding code = new Coding();
                code.Code = "410668003";
                concept.Coding.Add(code);
            }
            else if (conversionList.Text.Equals("bmr")) 
            {
                val.Data = CurrentMeasurement.bmr.ToString();
                Coding code = new Coding();
                code.Code = "60621009";
                concept.Coding.Add(code);
            }
            else if (conversionList.Text.Equals("height")) 
            {
                val.Data = CurrentMeasurement.height.ToString();
                Coding code = new Coding();
                code.Code = "60621009";
                concept.Coding.Add(code);
            }
            else if (conversionList.Text.Equals("m_active_time")) val.Data = CurrentMeasurement.m_active_time.ToString();
            else if (conversionList.Text.Equals("m_calories")) val.Data = CurrentMeasurement.m_calories.ToString();
            else if (conversionList.Text.Equals("m_distance")) val.Data = CurrentMeasurement.m_distance.ToString();
            else if (conversionList.Text.Equals("m_inactive_time")) val.Data = CurrentMeasurement.m_inactive_time.ToString();
            else if (conversionList.Text.Equals("m_lcat")) val.Data = CurrentMeasurement.m_lcat.ToString();
            else if (conversionList.Text.Equals("m_lcit")) val.Data = CurrentMeasurement.m_lcit.ToString();
            else if (conversionList.Text.Equals("m_steps")) val.Data = CurrentMeasurement.m_steps.ToString();
            else if (conversionList.Text.Equals("m_total_calories")) val.Data = CurrentMeasurement.m_total_calories.ToString();
            else if (conversionList.Text.Equals("s_asleep_time")) val.Data = CurrentMeasurement.s_asleep_time.ToString();
            else if (conversionList.Text.Equals("s_awake")) val.Data = CurrentMeasurement.s_awake.ToString();
            else if (conversionList.Text.Equals("s_awake_time")) val.Data = CurrentMeasurement.s_awake_time.ToString();
            else if (conversionList.Text.Equals("s_awakenings")) val.Data = CurrentMeasurement.s_awakenings.ToString();
            else if (conversionList.Text.Equals("s_bedtime")) val.Data = CurrentMeasurement.s_bedtime.ToString();
            else if (conversionList.Text.Equals("s_clinical_deep")) val.Data = CurrentMeasurement.s_clinical_deep.ToString();
            else if (conversionList.Text.Equals("s_count")) val.Data = CurrentMeasurement.s_count.ToString();
            else if (conversionList.Text.Equals("s_duration")) val.Data = CurrentMeasurement.s_duration.ToString();
            else if (conversionList.Text.Equals("s_light")) val.Data = CurrentMeasurement.s_light.ToString();
            else if (conversionList.Text.Equals("s_quality")) val.Data = CurrentMeasurement.s_quality.ToString();
            else if (conversionList.Text.Equals("s_rem")) val.Data = CurrentMeasurement.s_rem.ToString();
            else if (conversionList.Text.Equals("weight")) val.Data = CurrentMeasurement.weight.ToString();
            else val.Data = "E"; // Error
            

            Console.WriteLine("Value data: " + val.Data);

            ResourceReference dev = new ResourceReference();
            dev.Reference = CurrentDevice.ToString();

            ResourceReference pat = new ResourceReference();
            pat.Reference = CurrentPatient.ToString();

            Console.WriteLine("Patient reference: " + pat.Reference);

            Console.WriteLine("Conversion: " + conversionList.Text);

            DateTime date = DateTime.ParseExact(CurrentMeasurement.date, "yyyymmdd", null);
            Console.WriteLine("" + date.ToString());
            var obs = new Observation() {Device = dev, Subject = pat, Value = val, Issued = date,  Category = concept, Status = Observation.ObservationStatus.Final};
            client.Create(obs);

            conversionStatus.Text = "Done!";
        }

        private void obsSearchButton_Click(object sender, EventArgs e)
        {
            obsSearchStatus.Text = "Searching...";

            var endpoint = new Uri("http://spark-dstu2.furore.com/fhir");
            var client = new FhirClient(endpoint);

            var query = new string[] { "issued=" + obsSearchDate.Text };
            Bundle result = client.Search<Observation>(query);

            obsSearchStatus.Text = "Got " + result.Entry.Count() + " records!";

            obsFinderId.Text = "";
            obsFinderValue.Text = "";

            foreach (var entry in result.Entry)
            {
                Observation obs = (Observation)entry.Resource;

                obsFinderId.Text = obsFinderId.Text + obs.Id + "\r\n";
                obsFinderValue.Text = obsFinderValue.Text + obs.Value.ToString() + "\r\n";
            }
        }



    }
}
