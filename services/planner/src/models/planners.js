'use strict';
const orm = require('nodeorm');
const Model = orm.Model;


module.exports = new Model('planners', {
    owner: Number,
    name: String,
    description: String,
    creationTime: String
});
