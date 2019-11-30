const express = require("express");
const app = express();
const cookieParser = require("cookie-parser");
const bodyParser = require('body-parser');
const orm = require('node-orm-fluent');
const config = require('./config');
var swig = require('swig');
var swig = new swig.Swig();


orm.setConfig(config.db);

app.engine('html', swig.renderFile);
app.set('view engine', 'html');

app.use('/static', express.static(__dirname + '/views/static'));
app.use('/favicon.ico', express.static(__dirname + 'imgs/favicon.ico'));
app.set("orm", orm);
app.use(cookieParser());
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));
app.use(require('./routes'));

app.listen(config.service.port, function() {
    console.log(`[+] Service started on ${config.service.port} port`)
});

/*
app.get("/", async function(request, response, next) {
    return response.sendStatus(200);
});

app.get("/add", async function(request, response) {
    const res = await orm.tables.users.add({name:'kot', password: 'kotpass', n:123});
    return response.send(`ID is ${res}`);
});

app.get("/filter", async function(request, response) {
    const res = await orm.tables.users.filter({name:'kot'});
    console.log(res);
    response.send(`RES: ${res}`);
});

app.get("/remove", async function(request, response) {
    const res = await orm.tables.users.remove({name:'kot'});
    console.log(res);
    response.send(`RES: ${res}`);
});

app.get("/update", async function(request, response) {
    const res = await orm.tables.users.update({name:'tok'},{name:'ok'});
    response.send(`RES: ${res}`);
}); 
*/