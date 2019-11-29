var router = require('express').Router();

router.get('/', async function(req, resp, next) {
    resp.clearCookie('auth');
    resp.redirect('/');
});

module.exports = router;