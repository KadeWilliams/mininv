import "../js/jquery.import.js";

import "../../node_modules/jquery-ui/dist/jquery-ui.min.js"

import bootstrap from "../../node_modules/@hobbylobby/ui-toolkit-bootstrap/dist/js/bootstrap.bundle.js";
window.bootstrap = bootstrap;

import tomSelect from "../../node_modules/tom-select/dist/js/tom-select.complete.js";
window.TomSelect = tomSelect;

import flatpickr from "../../node_modules/flatpickr/dist/flatpickr.min.js"
import monthSelectPlugin from '../../node_modules/flatpickr/dist/plugins/monthSelect/index.js'

window.flatpickr = flatpickr;
window.monthSelectPlugin = monthSelectPlugin;

import jsZip from '../../node_modules/jszip/dist/jszip.min.js';
window.JSZip = jsZip;

import dataTable from "../../node_modules/datatables.net-bs5";
import '../../node_modules/datatables.net-buttons/js/dataTables.buttons.min.js';
import '../../node_modules/datatables.net-buttons-bs5/js/buttons.bootstrap5.min.js';
import '../../node_modules/datatables.net-buttons/js/buttons.html5.min.js';

$.fn.DataTable = dataTable;
window.DataTable = dataTable;

import { ValidationService } from 'aspnet-client-validation';
var validationService = new ValidationService();
validationService.bootstrap({ watch: true });
window.validationService = validationService;

import * as ProgressBar from '../../node_modules/progressbar.js/dist/progressbar.js'
window.ProgressBar = ProgressBar;

import * as request from "../../Views/Shared/Scripts/request.js";
window.request = request;


