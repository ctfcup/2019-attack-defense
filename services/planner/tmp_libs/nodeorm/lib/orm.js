'use strict';

const db = require('./db');
const Model = require('./model');

exports = module.exports;
exports.Model = Model;
exports.setConfig = (params) => { db.setConfig(params) };
exports.tables = db.tables;
exports.end = () => { db.end() };

