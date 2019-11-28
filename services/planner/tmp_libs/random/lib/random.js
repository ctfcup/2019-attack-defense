'use strict';

class Random 
{
    constructor(seed) {
        this.seed = seed;
        this.g = this.getGen(this.seed);
        for (var i=0; i<100; i++) this.g.next();
    }
    
    * getGen(seed) 
    {
        let p = [0];
        (seed >>> 0)
            .toString(2)
            .split('')
            .reverse()
            .forEach( function(e, i) { if (e == '1' && i != 0) p.push(i)});
        const r_len = p[p.length-1];
        let reg = 1;
        while(true) 
        {
            reg = ((p.slice(1,p.length).reduce(function(pv, cv, i) { return pv ^ (reg >>> cv) }, (reg >>> p[0])) & 1 ) << r_len) | (reg >>> 1);
            yield reg & 1;
        }
    }
        
    getInt() 
    {
        var res = [];
        for (var i=0; i<32; i++)
            res.push(this.g.next().value);
        return parseInt(res.join(''),2);
    }
    
    getHex() 
    {
        return this.getInt().toString(16).padStart(8, '0');
    }
    
    getHexBytes(len)
    {
        var res = [];
        for (; len>0; len--)
            res.push(this.getHex());
        return res.join('');
    }
}
exports = module.exports;
exports.Random = Random;

