module.exports = function (grunt) {
    grunt.initConfig({
        sass: {
            dist: {
                options: {
                    style: 'compact'
                },
                files: {
                    'css/styles.css': 'scss/styles.scss',
                }
            }
        },
        watch: {
            src: {
                files: ['js/*.js', 'scss/**/*.scss'],
                tasks: ['sass']
            }
        }
    });
    grunt.loadNpmTasks('grunt-contrib-sass');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.registerTask('default', ['watch']);
};
