import { HttpClient } from '@angular/common/http';
import { AfterViewInit, Component, OnInit } from '@angular/core';
import Chart from 'chart.js/auto';
import { FrequencyChartService } from '../../services/frequency-chart/frequency-chart.service';
import {formatDate} from '@angular/common';
import { FreqReadingDto } from 'src/app/models/freq-reading-dto';
import { ActivationEnd } from '@angular/router';

@Component({
  selector: 'app-frequency-chart',
  templateUrl: './frequency-chart.component.html',
  styleUrls: ['./frequency-chart.component.css']
})

export class FrequencyChartComponent implements OnInit {
  public chart: any
  public errorMessage: string|undefined;
  constructor(public service: FrequencyChartService) {}

  loadChart(): void {
    this.service.getFrequencyChartInfo(60).subscribe((response) => {

      if(response == undefined) 
      {
        this.errorMessage = "Fehler beim Abruf der Daten";
      } else {
        this.errorMessage = undefined;
        const labeldata: any[] = response?.map((x, i) => i == 0 ? 'jetzt' : formatDate(new Date(x.Timestamp), "HH:mm:ss", "de-DE").toString());
        const realdata: any[] = response?.map(x => x.Frequency);
        var showEmergencyMeasures = false;
        showEmergencyMeasures = realdata.some(x => (x > 51.5 || x < 49));
        this.createChart(labeldata.reverse(), realdata.reverse(), showEmergencyMeasures);
      }
    });
  }

  ngOnInit(): void {
    setInterval(() => this.loadChart(), 1000);
    this.loadChart();
  }

  createChart(labeldata: any, realdata: any, showEmergencyMeasures: boolean){
    if (this.chart == null) {
      this.chart = new Chart("FrequencyChart", {
        type: 'line', 
        data: null as any,
        options: {
          animation: {
            duration: 0,
          },
          interaction: {
            intersect: false
          },
          aspectRatio:2.5,
        },
      }
    )
    }

    var datasetData = [
      {
        label: "Netzfrequenz",
        data: realdata,
        backgroundColor: "rgba(0,0,0.7)",
        borderColor: "rgba(0,0,0,0.6)"
      },
      {
        label: "Richtwert",
        data: Array.apply(null, realdata).map(Number.prototype.valueOf, 50),
        fill: false,
        pointRadius: 0.1,
        borderColor: "rgba(50,205,50,0.8)",
        backgroundColor: "rgba(50,205,50,0.8)",
      },
      {
        label: "Start Regelleistung (negativ)",
        data: Array.apply(null, realdata).map(Number.prototype.valueOf, 50.02),
        fill: false,
        pointRadius: 0.1,
        borderColor: "rgba(255,165,0,0.5)",
        backgroundColor: "rgb(251, 244, 226)",
        borderDash: [5]
      },
      {
        label: "Start Regelleistung (positiv)",
        data: Array.apply(null, realdata).map(Number.prototype.valueOf, 49.98),
        fill: false,
        pointRadius: 0.1,
        borderColor: "rgba(255,165,0,0.5)",
        backgroundColor: "rgb(251, 244, 226)",
        borderDash: [20, 5]
      },
      {
        label: "Reduzierung der Einspeisung",
        data: Array.apply(null, realdata).map(Number.prototype.valueOf, 50.2),
        fill: false,
        pointRadius: 0.1,
        borderColor: "rgba(255,99,71,0.7)",
        backgroundColor: "rgb(251, 244, 226)",
        borderDash: [5]
      },
      {
        label: "Einspeisung von Leistungsreserven",
        data: Array.apply(null, realdata).map(Number.prototype.valueOf, 49.8),
        fill: false,
        pointRadius: 0.1,
        borderColor: "rgba(255,99,71,0.7)",
        backgroundColor: "rgb(251, 244, 226)",
        borderDash: [20, 5]
      },
    ]

    if (showEmergencyMeasures) {
      var blackoutsData = 
        {
          label: "Mögliche Stromausfälle",
          data: Array.apply(null, realdata).map(Number.prototype.valueOf, 51.5),
          fill: false,
          pointRadius: 0.1,
          borderColor: "rgba(255, 0, 0, 0.9)",
          backgroundColor: "rgb(251, 244, 226)",
          borderDash: [5],
        }
      var disconnectData = 
        {
          label: "Trennung aller Solaranlagen vom Netz",
          data: Array.apply(null, realdata).map(Number.prototype.valueOf, 49.0),
          fill: false,
          pointRadius: 0.1,
          borderColor: "rgba(255, 0, 0, 0.9)",
          backgroundColor: "rgb(251, 244, 226)",
          borderDash: [20, 5],
        }
      datasetData.push(blackoutsData, disconnectData)
    }

    this.chart.data = {
      labels: labeldata, 
      datasets: datasetData,
    };
    this.chart.update();
  }
}
